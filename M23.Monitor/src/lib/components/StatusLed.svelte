<script lang="ts">
    type LedState = 'stopped' | 'ready' | 'running' | 'fault';

    let { state }: { state: LedState } = $props();

    const colors: Record<LedState, string> = {
        stopped: 'bg-gray-300',
        ready: 'bg-amber-400',
        running: 'bg-green-500',
        fault: 'bg-red-500'
    };
</script>



<span
    class="inline-block w-3 h-3 rounded-full {colors[state]} {state === 'fault' ? 'animate-pulse' : ''}"
></span>