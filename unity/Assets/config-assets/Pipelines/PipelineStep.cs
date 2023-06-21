using System;

namespace ConfigAssets.Pipelines {
    public class PipelineStep : IPipelineStep {
        private readonly Action _action;

        public PipelineStep(Action action) {
            _action = action;
        }

        public bool Run() {
            _action();

            return true;
        }
    }
}