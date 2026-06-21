<script lang="ts">
    import type { SystemState } from '$lib/types';

    let { state, alarmActive }: { state: SystemState; alarmActive: boolean } = $props();

    const styles: Record<SystemState, string> = {
        Idle: 'bg-gray-100 text-gray-700 border-gray-200',
        Normal: 'bg-green-50 text-green-800 border-green-200',
        Fault: 'bg-red-50 text-red-800 border-red-200'
    };
</script>



<div class="rounded-lg border px-4 py-2.5 flex items-center justify-between {styles[state]}">
    <span class="text-sm font-medium">System: {state}</span>
    {#if alarmActive}
        <span class="text-xs font-medium animate-pulse">⚠ Alarm sounding</span>
    {/if}
</div>