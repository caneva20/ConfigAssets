using System;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Demo {
    [Config(DisplayName = "ConfigAssets demo")]
    public partial class EnhanceDemo {
        [SerializeField] private string _myKeyToSomeService;
        [SerializeField] private string[] _listOfRandomStrings;
        public int _theQuantityOfSomeStuff;

        [SerializeField] private MyOtherClass _myOtherClassThatsSerializable;
    }

    [Serializable]
    public class MyOtherClass {
        [SerializeField] private string _theNameOfSomething;
        public int andItsAge;
    }
}
