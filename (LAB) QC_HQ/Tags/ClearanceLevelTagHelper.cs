/* NOTES 
 
Mostly tried it out to see functionality that it provides for Razor views.
It's very helpfull to create custom tags that can be reused across the application. 
Used exclusively for displaying clearance levels in a standardized way as of now, may be expanded in the future.
 
 */

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("clearance-level")]
public class ClearanceLevelTagHelper : TagHelper
{
    public byte Level { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.Attributes.SetAttribute("class", $"clearance-level-{Level}");
        output.Content.SetHtmlContent(context.AllAttributes["level"]?.Value.ToString() ?? Level.ToString());
    }
}