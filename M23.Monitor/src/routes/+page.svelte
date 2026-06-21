<script lang="ts">
    import { connection } from '$lib/stores/connection.svelte';
    import ProcessSchematic from '$lib/components/ProcessSchematic.svelte';
    import BeltCard from '$lib/components/BeltCard.svelte';
    import ControlPanel from '$lib/components/ControlPanel.svelte';
    import SystemBanner from '$lib/components/SystemBanner.svelte';

    const state = $derived(connection.state);
</script>



<div class="max-w-5xl mx-auto space-y-4">
    <SystemBanner state={state.system} alarmActive={state.alarmActive} />

    <div class="grid md:grid-cols-2 gap-4">
        <div class="bg-white border border-gray-200 rounded-lg p-4">
            <ProcessSchematic {state} />
        </div>

        <div class="space-y-3">
            <div class="grid grid-cols-2 gap-2">
                <BeltCard belt="M1" state={state.belts.M1} isInput={true} />
                <BeltCard belt="M2" state={state.belts.M2} isInput={true} />
                <BeltCard belt="M3" state={state.belts.M3} isInput={false} />
                <BeltCard belt="M4" state={state.belts.M4} isInput={false} />
            </div>

            <ControlPanel {state} />
        </div>
    </div>
</div>