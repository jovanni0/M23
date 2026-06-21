import type { InputBeltState, OutputBeltState, FlapPosition } from '$lib/types';



export function ledState(state: InputBeltState | OutputBeltState): 'stopped' | 'ready' | 'running' {
    if (state === 'Running') return 'running';
    if (state === 'Ready') return 'ready';
    return 'stopped';
}



export function flapAngle(position: FlapPosition): number {
    switch (position) {
        case 'S6': return -25;
        case 'S8': return 25;
        default: return 0;
    }
}



export function isBeltAnimated(state: InputBeltState | OutputBeltState): boolean {
    return state === 'Running';
}