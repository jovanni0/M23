<script lang="ts">
    import { fetchFaultPeriods } from '$lib/api';
    import type { FaultPeriod } from '$lib/types';
    import { onMount } from 'svelte';


    let periods = $state<FaultPeriod[]>([]);
    let loading = $state(true);
    let error = $state<string | null>(null);

    async function load() {
        loading = true;
        error = null;
        try {
            periods = await fetchFaultPeriods();
        } catch (e) {
            error = e instanceof Error ? e.message : 'Failed to load reports';
        } finally {
            loading = false;
        }
    }

    onMount(load);

    function formatTime(value: string): string {
        return new Date(value).toLocaleString();
    }

    function formatDuration(value: string | null): string {
        if (!value) return 'Ongoing';
        // .NET TimeSpan serializes as "hh:mm:ss" or "d.hh:mm:ss"
        return value;
    }
</script>



<div class="max-w-3xl mx-auto space-y-4">
    <div class="flex items-center justify-between">
        <h2 class="text-base font-medium text-gray-900">Fault report</h2>
        <button
                onclick={load}
                class="text-xs px-2 py-1 border border-gray-300 rounded hover:bg-gray-50"
        >
            Refresh
        </button>
    </div>

    {#if loading}
        <p class="text-sm text-gray-400 py-8 text-center">Loading…</p>
    {:else if error}
        <p class="text-sm text-red-600 py-8 text-center">{error}</p>
    {:else if periods.length === 0}
        <p class="text-sm text-gray-400 py-8 text-center">No faults recorded</p>
    {:else}
        <div class="bg-white border border-gray-200 rounded-lg overflow-hidden">
            <table class="w-full text-sm">
                <thead class="bg-gray-50 text-gray-500 text-xs">
                <tr>
                    <th class="text-left px-3 py-2 font-medium">Faulted at</th>
                    <th class="text-left px-3 py-2 font-medium">Resolved at</th>
                    <th class="text-left px-3 py-2 font-medium">Duration</th>
                </tr>
                </thead>
                <tbody class="divide-y divide-gray-100">
                {#each periods as period}
                    <tr>
                        <td class="px-3 py-2">{formatTime(period.faultedAt)}</td>
                        <td class="px-3 py-2">
                            {#if period.resolvedAt}
                                {formatTime(period.resolvedAt)}
                            {:else}
                                <span class="text-red-600 font-medium">Unresolved</span>
                            {/if}
                        </td>
                        <td class="px-3 py-2">{formatDuration(period.duration)}</td>
                    </tr>
                {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>