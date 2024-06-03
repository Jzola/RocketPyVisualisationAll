using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

public class GeneratedDataSource : DataSource
{
    public override bool IsLoaded
    {
        get { return DimensionCount > 0; }
    }

    public override int DimensionCount => throw new System.NotImplementedException();

    public override int DataCount => throw new System.NotImplementedException();

    public override DimensionData this[int index] => throw new System.NotImplementedException();

    public override DimensionData this[string identifier] => throw new System.NotImplementedException();

    public override object getOriginalValue(float normalisedValue, string identifier) => throw new System.NotImplementedException();

    /// <summary>
    /// Returns the original value of a data dimension normalised value
    /// </summary>
    /// <param name="normalisedValue"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public override object getOriginalValue(float normalisedValue, int identifier) => throw new System.NotImplementedException();

    /// <summary>
    /// gets the original from the exact normaliserdValue matching value
    /// </summary>
    /// <param name="normalisedValue"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public override object getOriginalValuePrecise(float normalisedValue, int identifier) => throw new System.NotImplementedException();

    /// <summary>
    /// gets the original from the exact normaliserdValue matching value
    /// </summary>
    /// <param name="normalisedValue"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public override object getOriginalValuePrecise(float normalisedValue, string identifier) => throw new System.NotImplementedException();


    /// <summary>
    /// Returns the number of categories for a data dimensions
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public override int getNumberOfCategories(int identifier) => throw new System.NotImplementedException();

    /// <summary>
    /// Returns the number of categories for a data dimensions
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public override int getNumberOfCategories(string identifier) => throw new System.NotImplementedException();

    /// <summary>
    /// Load the header information for the data
    /// Available here so can be called in editor
    /// Post: The Identifier and Metadata.type values will be available for each dimension
    /// </summary>
    public override void loadHeader() => throw new System.NotImplementedException();

    /// <summary>
    /// Load the data
    /// </summary>
    public override void load() => throw new System.NotImplementedException();
}
