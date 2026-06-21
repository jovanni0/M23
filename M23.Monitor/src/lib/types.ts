// src/lib/types.ts

export type SystemState = 'Idle' | 'Normal' | 'Fault';
export type FlapPosition = 'S6' | 'S7' | 'S8' | 'Fault';
export type InputBeltState = 'Stopped' | 'Ready' | 'Running';
export type OutputBeltState = 'Stopped' | 'Running';

export interface BeltStates {
    M1: InputBeltState;
    M2: InputBeltState;
    M3: OutputBeltState;
    M4: OutputBeltState;
}

export interface ProcessState {
    system: SystemState;
    flap: FlapPosition;
    belts: BeltStates;
    alarmActive: boolean;
}

export type WsMessageType =
    | 'sync'
    | 'belt_state'
    | 'flap_state'
    | 'system_state'
    | 'alarm'
    | 'command';

export interface SyncPayload {
    system: SystemState;
    flap: FlapPosition;
    belts: BeltStates;
}

export interface BeltStatePayload {
    belt: keyof BeltStates;
    state: InputBeltState | OutputBeltState;
}

export interface FlapStatePayload {
    position: FlapPosition;
}

export interface SystemStatePayload {
    state: SystemState;
}

export interface AlarmPayload {
    active: boolean;
}

export interface CommandPayload {
    target: string;
    action: string;
    position?: FlapPosition;
}

export type WsMessage =
    | { type: 'sync'; payload: SyncPayload }
    | { type: 'belt_state'; payload: BeltStatePayload }
    | { type: 'flap_state'; payload: FlapStatePayload }
    | { type: 'system_state'; payload: SystemStatePayload }
    | { type: 'alarm'; payload: AlarmPayload }
    | { type: 'command'; payload: CommandPayload };

export interface EventRecord {
    source: string;
    from: string;
    to: string;
    timestamp: string;
}

export interface FaultPeriod {
    faultedAt: string;
    resolvedAt: string | null;
    duration: string | null;
}