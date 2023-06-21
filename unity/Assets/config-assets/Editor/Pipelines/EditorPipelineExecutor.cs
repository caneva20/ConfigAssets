using System.Collections;
using System.Collections.Generic;
using ConfigAssets.Pipelines;
using Unity.EditorCoroutines.Editor;

namespace ConfigAssets.Editor.Pipelines {
    public class EditorPipelineExecutor : IPipelineExecutor {
        public void Process(float delay, IEnumerable<IPipelineStep> steps) {
            EditorCoroutineUtility.StartCoroutineOwnerless(ProcessEnumerator(delay, steps));
        }

        public IEnumerator ProcessEnumerator(float delay, IEnumerable<IPipelineStep> steps) {
            foreach (var step in steps) {
                if (step.Run()) {
                    yield return new EditorWaitForSeconds(delay);
                }
            }
        }
    }
}