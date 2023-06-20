using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;

namespace ConfigAssets.Pipelines {
    public static class ProcessingPipeline {
        public static void Process(float delay, IEnumerable<IPipelineStep> steps) {
            EditorCoroutineUtility.StartCoroutineOwnerless(ProcessEnumerator(delay, steps));
        }

        public static IEnumerator ProcessEnumerator(float delay, IEnumerable<IPipelineStep> steps) {
            foreach (var step in steps) {
                if (step.Run()) {
                    yield return new EditorWaitForSeconds(delay);
                }
            }
        }
    }
}