<script lang="ts">
    import type { SystemState } from '$lib/types';

    let { state, alarmActive }: { state: SystemState; alarmActive: boolean } = $props();

    const styles: Record<SystemState, string> = {
        Idle: 'bg-gray-100 text-gray-700 border-gray-200',
        Normal: 'bg-green-50 text-green-800 border-green-200',
        Fault: 'bg-red-50 text-red-800 border-red-200'
    };

    let audioCtx: AudioContext | null = null;
    let oscillator: OscillatorNode | null = null;
    let gainNode: GainNode | null = null;
    let lfoInterval: ReturnType<typeof setInterval> | null = null;

    function startAlarm() {
        stopAlarm();
        try {
            audioCtx = new AudioContext();
            oscillator = audioCtx.createOscillator();
            gainNode = audioCtx.createGain();

            oscillator.type = 'square';
            oscillator.frequency.setValueAtTime(800, audioCtx.currentTime);
            gainNode.gain.setValueAtTime(0.15, audioCtx.currentTime);

            oscillator.connect(gainNode);
            gainNode.connect(audioCtx.destination);
            oscillator.start();

            // Alternate between 800Hz and 600Hz for urgent alarm feel
            let high = true;
            lfoInterval = setInterval(() => {
                if (oscillator && audioCtx) {
                    high = !high;
                    oscillator.frequency.setValueAtTime(
                        high ? 800 : 600,
                        audioCtx.currentTime
                    );
                }
            }, 250);
        } catch {
            // Audio may not be available in all environments
        }
    }

    function stopAlarm() {
        if (lfoInterval) {
            clearInterval(lfoInterval);
            lfoInterval = null;
        }
        if (oscillator) {
            try { oscillator.stop(); } catch { /* already stopped */ }
            oscillator = null;
        }
        if (gainNode) {
            gainNode.disconnect();
            gainNode = null;
        }
        if (audioCtx) {
            audioCtx.close();
            audioCtx = null;
        }
    }

    $effect(() => {
        if (alarmActive) {
            startAlarm();
        } else {
            stopAlarm();
        }
        return () => stopAlarm();
    });
</script>



<div class="rounded-lg border px-4 py-2.5 flex items-center justify-between {styles[state]}">
    <span class="text-sm font-medium">System: {state}</span>
    {#if alarmActive}
        <span class="text-xs font-medium animate-pulse">⚠ Alarm sounding</span>
    {/if}
</div>