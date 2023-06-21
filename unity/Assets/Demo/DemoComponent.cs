using UnityEngine;
using UnityEngine.UI;

namespace ConfigAssets.Demo {
    public class DemoComponent : MonoBehaviour {
        [SerializeField] private Image _image;
        
        private void Start() {
            _image.color = DemoConfig.Color;
        }
    }
}