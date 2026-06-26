<script lang="ts">
    import StatusLed from './StatusLed.svelte';
    import { ledState } from '$lib/beltLogic';
    import { connection } from '$lib/stores/connection.svelte';
    import type { InputBeltState, OutputBeltState } from '$lib/types';

    let {
        belt,
        state,
        isInput
    }: {
        belt: string;
        state: InputBeltState | OutputBeltState;
        isInput: boolean;
    } = $props();

    function toggle() {
        if (state === 'Stopped') {
            connection.startBelt(belt);
        } else {
            connection.stopBelt(belt);
        }
    }
</script>



<div class="bg-white border border-gray-200 rounded-lg p-3 flex items-center justify-between">
    <div class="flex items-center gap-2">
        <StatusLed state={ledState(state)} />
        <div>
            <p class="text-sm font-medium text-gray-900">{belt}</p>
            <p class="text-xs text-gray-500">{state}{isInput ? '' : ' (output)'}</p>
        </div>
    </div>
    <button
            onclick={toggle}
            disabled={connection.state.system === 'Fault'}
            class="text-xs px-2.5 py-1 border border-gray-300 rounded hover:bg-gray-50
            disabled:opacity-40 disabled:cursor-not-allowed"
    >
        {state === 'Stopped' ? 'Start' : 'Stop'}
    </button>
</div>