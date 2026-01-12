using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("clearance-level")]
public class ClearanceLevelTagHelper : TagHelper
{
    public byte Level { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.Attributes.SetAttribute("class", $"clearance-level-{Level}");

        // KEEP whatever content is inside the tag
        // Don't change the text at all
        output.Content.SetHtmlContent(context.AllAttributes["level"]?.Value.ToString() ?? Level.ToString());
    }
}