using System;

namespace ConfigAssets.Pipelines {
    public class ConditionalPipelineStep : IPipelineStep {
        private readonly Func<bool> _condition;
        private readonly Action _action;

        public ConditionalPipelineStep(Func<bool> condition, Action action) {
            _condition = condition;
            _action = action;
        }

        public bool Run() {
            if (!_condition()) {
                return false;
            }

            _action();

            return true;
        }
    }
}