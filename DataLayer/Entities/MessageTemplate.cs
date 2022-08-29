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
	[Table("MessageTemplate")]
	public class MessageTemplate
	{
		[Column("MessageTemplateId"                   )] public long   MessageTemplateId { get; set; } // integer
		[Column("Subject"          , CanBeNull = false)] public string Subject           { get; set; } = null!; // text(max)
		[Column("Body"             , CanBeNull = false)] public string Body              { get; set; } = null!; // text(max)
		[Column("ContentType"      , CanBeNull = false)] public string ContentType       { get; set; } = null!; // text(max)
		[Column("IsBodyStatic"                        )] public bool   IsBodyStatic      { get; set; } // boolean
		[Column("IsSubjectStatic"                     )] public bool   IsSubjectStatic   { get; set; } // boolean
		[Column("ModelProviderId"                     )] public long?  ModelProviderId   { get; set; } // integer
	}
}