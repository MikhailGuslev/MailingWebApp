// ---------------------------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by LinqToDB scaffolding tool (https://github.com/linq2db/linq2db).
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
// ---------------------------------------------------------------------------------------------------

using LinqToDB.Mapping;

#pragma warning disable 1573, 1591
#nullable enable

namespace DataLayer.Entities
{
	[Table("ModelProvider")]
	public class ModelProvider
	{
		[Column("ModelProviderId"                         )] public long   ModelProviderId       { get; set; } // integer
		[Column("ModelProviderTypeName", CanBeNull = false)] public string ModelProviderTypeName { get; set; } = null!; // text(max)
		[Column("PluginId"                                )] public long   PluginId              { get; set; } // integer
	}
}
