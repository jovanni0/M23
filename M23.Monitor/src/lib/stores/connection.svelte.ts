import type {
    ProcessState,
    WsMessage,
    CommandPayload,
    FlapPosition
} from '$lib/types';

const DEFAULT_STATE: ProcessState = {
    system: 'Idle',
    flap: 'S7',
    belts: { M1: 'Stopped', M2: 'Stopped', M3: 'Stopped', M4: 'Stopped' },
    alarmActive: false
};

type ConnectionStatus = 'disconnected' | 'connecting' | 'connected' | 'error';

function createConnection() {
    let socket: WebSocket | null = null;
    let reconnectTimer: ReturnType<typeof setTimeout> | null = null;

    let state = $state<ProcessState>({ ...DEFAULT_STATE });
    let status = $state<ConnectionStatus>('disconnected');
    let events = $state<{ source: string; from: string; to: string; timestamp: Date }[]>([]);
    let alarmTimer: ReturnType<typeof setTimeout> | null = null;
    let perturbationEnabled = $state<boolean>(true);

    function connect(url: string) {
        if (socket) socket.close();
        status = 'connecting';

        socket = new WebSocket(url);

        socket.onopen = () => {
            status = 'connected';
            if (reconnectTimer) clearTimeout(reconnectTimer);
        };

        socket.onmessage = (event) => {
            try {
                const message: WsMessage = JSON.parse(event.data);
                handleMessage(message);
            } catch {
                console.error('Failed to parse message:', event.data);
            }
        };

        socket.onclose = () => {
            status = 'disconnected';
            reconnectTimer = setTimeout(() => connect(url), 3000);
        };

        socket.onerror = () => {
            status = 'error';
        };
    }

    function handleMessage(message: WsMessage) {
        switch (message.type) {
            case 'sync':
                state = {
                    ...message.payload,
                    alarmActive: false
                };
                break;

            case 'belt_state':
                state = {
                    ...state,
                    belts: {
                        ...state.belts,
                        [message.payload.belt]: message.payload.state
                    }
                };
                break;

            case 'flap_state':
                state = { ...state, flap: message.payload.position };
                break;

            case 'system_state':
                state = { ...state, system: message.payload.state };
                if (message.payload.state === 'Fault') {
                    state = { ...state, alarmActive: true };
                    if (alarmTimer) clearTimeout(alarmTimer);
                    alarmTimer = setTimeout(() => {
                        state = { ...state, alarmActive: false };
                        alarmTimer = null;
                    }, 5000);
                } else {
                    if (alarmTimer) {
                        clearTimeout(alarmTimer);
                        alarmTimer = null;
                    }
                    state = { ...state, alarmActive: false };
                }
                break;

            case 'alarm':
                state = { ...state, alarmActive: message.payload.active };
                break;
        }

        // Track perturbation toggle events
        if (message.type !== 'sync') {
            const src = getEventSource(message);
            const to = getEventTo(message);
            if (src === 'PERTURBATION') {
                perturbationEnabled = to === 'Enabled';
            }
        }

        if (message.type !== 'sync') {
            events = [
                {
                    source: getEventSource(message),
                    from: getEventFrom(message),
                    to: getEventTo(message),
                    timestamp: new Date()
                },
                ...events.slice(0, 199)
            ];
        }
    }

    function getEventSource(message: WsMessage): string {
        switch (message.type) {
            case 'belt_state': return message.payload.belt;
            case 'flap_state': return 'FLAP';
            case 'system_state': return 'SYSTEM';
            case 'alarm': return 'ALARM';
            default: return '';
        }
    }

    function getEventFrom(_message: WsMessage): string {
        return '';
    }

    function getEventTo(message: WsMessage): string {
        switch (message.type) {
            case 'belt_state': return message.payload.state;
            case 'flap_state': return message.payload.position;
            case 'system_state': return message.payload.state;
            case 'alarm': return message.payload.active ? 'Active' : 'Inactive';
            default: return '';
        }
    }

    function sendCommand(payload: CommandPayload) {
        if (!socket || socket.readyState !== WebSocket.OPEN) return;
        socket.send(JSON.stringify({ type: 'command', payload }));
    }

    function startBelt(belt: string) {
        sendCommand({ target: belt, action: 'start' });
    }

    function stopBelt(belt: string) {
        sendCommand({ target: belt, action: 'stop' });
    }

    function pressS0() {
        sendCommand({ target: 'S0', action: 'press' });
    }

    function pressS5() {
        sendCommand({ target: 'S5', action: 'press' });
    }

    function setFlap(position: FlapPosition) {
        sendCommand({ target: 'FLAP', action: 'set', position });
    }

    function triggerFault() {
        sendCommand({ target: 'FLAP', action: 'fault' });
    }

    function togglePerturbation() {
        perturbationEnabled = !perturbationEnabled;
        sendCommand({ target: 'PERTURBATION', action: 'toggle_perturbation' });
    }

    function restart() {
        sendCommand({ target: 'SYSTEM', action: 'restart' });
    }

    function disconnect() {
        if (reconnectTimer) clearTimeout(reconnectTimer);
        socket?.close();
        socket = null;
        status = 'disconnected';
    }

    return {
        get state() { return state; },
        get status() { return status; },
        get events() { return events; },
        get perturbationEnabled() { return perturbationEnabled; },
        connect,
        disconnect,
        startBelt,
        stopBelt,
        pressS0,
        pressS5,
        setFlap,
        triggerFault,
        togglePerturbation,
        restart
    };
}

export const connection = createConnection();