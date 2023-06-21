using System;
using UnityEngine;

namespace ConfigAssets.Demo {
    [Config(DisplayName = "ConfigAssets demo", Keywords = new[] { "Demo", "ConfigAssets" })]
    public partial class DemoConfig {
        [SerializeField] private string _myKeyToSomeService;
        [SerializeField] private string[] _listOfRandomStrings;
        public int _theQuantityOfSomeStuff;

        [SerializeField] private MyOtherClass _myOtherClassThatsSerializable;

        [SerializeField] private Color _color;
    }

    [Serializable]
    public class MyOtherClass {
        [SerializeField] private string _theNameOfSomething;
        public int andItsAge;
    }
}