using System.Collections.Generic;

namespace ConfigAssets.Pipelines {
    public class CombinedPipelineStep : IPipelineStep {
        private readonly IEnumerable<IPipelineStep> _steps;

        public CombinedPipelineStep(IEnumerable<IPipelineStep> steps) {
            _steps = steps;
        }

        public bool Run() {
            foreach (var step in _steps) {
                step.Run();
            }

            return true;
        }
    }
}