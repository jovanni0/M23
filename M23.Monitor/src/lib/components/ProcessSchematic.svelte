<script lang="ts">
    import type { ProcessState } from '$lib/types';
    import { ledState, flapAngle, isBeltAnimated } from '$lib/beltLogic';

    let { state }: { state: ProcessState } = $props();

    const ledColor = {
        stopped: '#D1D5DB',
        ready: '#FBBF24',
        running: '#22C55E'
    };
</script>



<svg viewBox="0 0 400 320" class="w-full h-auto max-w-md mx-auto">
    <!-- M1 input belt (top left) -->
    <g>
        <rect x="20" y="40" width="120" height="24" rx="4" fill="none" stroke="#9CA3AF" stroke-width="1.5"/>
        <line
                x1="24" y1="52" x2="136" y2="52"
                stroke="#6B7280" stroke-width="2" stroke-dasharray="6 6"
                class={isBeltAnimated(state.belts.M1) ? 'animate-belt' : ''}
        />
        <circle cx="130" cy="52" r="6" fill={ledColor[ledState(state.belts.M1)]} />
        <text x="80" y="34" text-anchor="middle" font-size="12" fill="#374151">M1</text>
    </g>

    <!-- M2 input belt (top right) -->
    <g>
        <rect x="260" y="40" width="120" height="24" rx="4" fill="none" stroke="#9CA3AF" stroke-width="1.5"/>
        <line
                x1="264" y1="52" x2="376" y2="52"
                stroke="#6B7280" stroke-width="2" stroke-dasharray="6 6"
                class={isBeltAnimated(state.belts.M2) ? 'animate-belt-reverse' : ''}
        />
        <circle cx="270" cy="52" r="6" fill={ledColor[ledState(state.belts.M2)]} />
        <text x="320" y="34" text-anchor="middle" font-size="12" fill="#374151">M2</text>
    </g>

    <!-- Flap (center) -->
    <g transform="translate(200, 140)">
        <circle r="10" fill="white" stroke="#374151" stroke-width="1.5" />
        <line
                x1="0" y1="0" x2="0" y2="-22"
                stroke="#374151" stroke-width="2.5" stroke-linecap="round"
                transform="rotate({flapAngle(state.flap)})"
                class="transition-transform duration-300"
        />
        <text x="0" y="34" text-anchor="middle" font-size="11" fill="#6B7280">
            {state.flap === 'Fault' ? 'FAULT' : state.flap}
        </text>
    </g>

    <!-- M3 output belt (bottom left) -->
    <g>
        <rect x="20" y="220" width="120" height="24" rx="4" fill="none" stroke="#9CA3AF" stroke-width="1.5"/>
        <line
                x1="24" y1="232" x2="136" y2="232"
                stroke="#6B7280" stroke-width="2" stroke-dasharray="6 6"
                class={isBeltAnimated(state.belts.M3) ? 'animate-belt-reverse' : ''}
        />
        <circle cx="30" cy="232" r="6" fill={ledColor[ledState(state.belts.M3)]} />
        <text x="80" y="262" text-anchor="middle" font-size="12" fill="#374151">M3</text>
    </g>

    <!-- M4 output belt (bottom right) -->
    <g>
        <rect x="260" y="220" width="120" height="24" rx="4" fill="none" stroke="#9CA3AF" stroke-width="1.5"/>
        <line
                x1="264" y1="232" x2="376" y2="232"
                stroke="#6B7280" stroke-width="2" stroke-dasharray="6 6"
                class={isBeltAnimated(state.belts.M4) ? 'animate-belt' : ''}
        />
        <circle cx="370" cy="232" r="6" fill={ledColor[ledState(state.belts.M4)]} />
        <text x="320" y="262" text-anchor="middle" font-size="12" fill="#374151">M4</text>
    </g>

    <!-- Connecting lines from belts to flap -->
    <line x1="130" y1="60" x2="195" y2="135" stroke="#D1D5DB" stroke-width="1.5" />
    <line x1="270" y1="60" x2="205" y2="135" stroke="#D1D5DB" stroke-width="1.5" />
    <line x1="195" y1="145" x2="130" y2="225" stroke="#D1D5DB" stroke-width="1.5" />
    <line x1="205" y1="145" x2="270" y2="225" stroke="#D1D5DB" stroke-width="1.5" />
</svg>



<style>
    @keyframes belt-move {
        from { stroke-dashoffset: 0; }
        to { stroke-dashoffset: -24; }
    }
    @keyframes belt-move-reverse {
        from { stroke-dashoffset: 0; }
        to { stroke-dashoffset: 24; }
    }
    :global(.animate-belt) {
        animation: belt-move 0.6s linear infinite;
    }
    :global(.animate-belt-reverse) {
        animation: belt-move-reverse 0.6s linear infinite;
    }
</style>