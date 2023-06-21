using System.Collections;
using System.Collections.Generic;

namespace ConfigAssets.Pipelines {
    public interface IPipelineExecutor {
        void Process(float delay, IEnumerable<IPipelineStep> steps);
        
        IEnumerator ProcessEnumerator(float delay, IEnumerable<IPipelineStep> steps);
    }
}