<script lang="ts">
    import { connection } from "$lib/stores/connection.svelte";
    import type { FlapPosition, SystemState } from "$lib/types";

    interface Props {
        state: { system: SystemState; flap: FlapPosition };
    }

    let { state }: Props = $props();

    const positions: FlapPosition[] = ["S6", "S7", "S8"];
</script>

<div class="bg-white border border-gray-200 rounded-lg p-3 space-y-3">
    <div>
        <p class="text-xs text-gray-500 mb-1.5">Flap position</p>
        <div class="flex gap-1.5">
            {#each positions as pos}
                <button
                    onclick={() => connection.setFlap(pos)}
                    disabled={state.system === "Fault"}
                    class="flex-1 text-xs py-1.5 rounded border
            {state.flap === pos
                        ? 'bg-gray-900 text-white border-gray-900'
                        : 'border-gray-300 hover:bg-gray-50'}
            disabled:opacity-40 disabled:cursor-not-allowed"
                >
                    {pos}
                </button>
            {/each}
        </div>
    </div>

    <div class="flex gap-1.5 pt-1 border-t border-gray-100">
        <button
            onclick={() => connection.pressS0()}
            class="flex-1 text-xs py-1.5 rounded bg-red-50 text-red-700 border border-red-200 hover:bg-red-100"
        >
            S0 — stop all
        </button>
        <button
            onclick={() => connection.pressS5()}
            class="flex-1 text-xs py-1.5 rounded bg-amber-50 text-amber-700 border border-amber-200 hover:bg-amber-100"
        >
            S5 — stop inputs
        </button>
    </div>

    {#if state.system === "Fault"}
        <button
            onclick={() => connection.restart()}
            class="w-full text-xs py-2 rounded bg-gray-900 text-white hover:bg-gray-800"
        >
            Restart system
        </button>
    {:else}
        <button
            onclick={() => connection.triggerFault()}
            disabled={state.system !== "Normal"}
            class="w-full text-xs py-1.5 rounded border border-gray-300 text-gray-500 hover:bg-gray-50
                disabled:opacity-40 disabled:cursor-not-allowed"
        >
            Trigger fault (test)
        </button>
    {/if}

    <div
        class="flex items-center justify-between pt-1 border-t border-gray-100"
    >
        <span class="text-xs text-gray-500">Random faults</span>
        <button
            aria-label="Toggle random faults"
            onclick={() => connection.togglePerturbation()}
            class="relative inline-flex h-5 w-9 items-center rounded-full transition-colors
                {connection.perturbationEnabled
                ? 'bg-green-500'
                : 'bg-gray-300'}"
        >
            <span
                class="inline-block h-3.5 w-3.5 rounded-full bg-white transition-transform
                    {connection.perturbationEnabled
                    ? 'translate-x-4'
                    : 'translate-x-0.5'}"
            ></span>
        </button>
    </div>
</div>
