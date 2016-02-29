using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common
{
    /// <summary>A general purpose delegate which does not take a parameter.</summary>
    public delegate void ParameterlessDelegate();

    /// <summary>A general purpose generic delegate which takes a single parameter.</summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="obj">The object to pass through the delegate.</param>
    public delegate void GenericDelegate<T>(T obj);

    /// <summary>A general purpose generic delegate which takes two parameters.</summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="obj1">The first object to pass through the delegate.</param>
    /// <param name="obj2">The second object to pass through the delegate.</param>
    public delegate void GenericDelegate<T1, T2>(T1 obj1, T2 obj2);

    /// <summary>A general purpose delegate which returns a string.</summary>
    /// <returns>A string.</returns>
    public delegate string GetStringValueDelegate();

    /// <summary>Encapsulates a method that takes a single parameter and does not return a value.</summary>
    public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);

    /// <summary>Encapsulates a method that takes a single parameter and does not return a value.</summary>
    public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);

    /// <summary>Encapsulates a method that takes a single parameter and does not return a value.</summary>
    public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);

    /// <summary>Encapsulates a method that takes a single parameter and does not return a value.</summary>
    public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8);
}
