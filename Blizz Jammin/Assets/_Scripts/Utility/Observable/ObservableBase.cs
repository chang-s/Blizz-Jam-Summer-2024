// https://github.com/Adam4lexander/UnityObservables

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Utility.Observable {

    public interface IObservable {
        event Action OnChanged;
    }

    // Base class implemented by all Observable types.
    public abstract class Observable : IObservable {
        public abstract event Action OnChanged;

        // Can be called in a MonoBehaviours OnValidate section so events can fire after an UNDO operation
        public abstract void OnValidate();

        public abstract string ValuePropName { get; }

        public abstract void OnBeginGui();
    }
}