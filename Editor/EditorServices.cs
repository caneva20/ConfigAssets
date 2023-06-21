using ConfigAssets.Editor.Pipelines;
using ConfigAssets.Pipelines;

namespace ConfigAssets.Editor {
    public static class EditorServices {
        public static readonly IPipelineExecutor PipelineExecutor = new EditorPipelineExecutor();
    }
}