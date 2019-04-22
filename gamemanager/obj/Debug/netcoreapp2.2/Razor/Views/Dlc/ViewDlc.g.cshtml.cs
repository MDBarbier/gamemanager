#pragma checksum "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "cd59f3ddaa7c1a38b91937d926286f87becca3c4"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Dlc_ViewDlc), @"mvc.1.0.view", @"/Views/Dlc/ViewDlc.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Dlc/ViewDlc.cshtml", typeof(AspNetCore.Views_Dlc_ViewDlc))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\_ViewImports.cshtml"
using gamemanager;

#line default
#line hidden
#line 2 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\_ViewImports.cshtml"
using gamemanager.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cd59f3ddaa7c1a38b91937d926286f87becca3c4", @"/Views/Dlc/ViewDlc.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"70fb08ffe92f4a0289e91bea68d7752a1b875ee1", @"/Views/_ViewImports.cshtml")]
    public class Views_Dlc_ViewDlc : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<GameEntry>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(17, 1, true);
            WriteLiteral("\n");
            EndContext();
#line 3 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
  
    ViewData["Title"] = "View Dlc";

#line default
#line hidden
            BeginContext(59, 58, true);
            WriteLiteral("\n<div class=\"jumbotron text-center\">\n    <h1>View DLC for ");
            EndContext();
            BeginContext(118, 10, false);
#line 8 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                Write(Model.Name);

#line default
#line hidden
            EndContext();
            BeginContext(128, 539, true);
            WriteLiteral(@"</h1>
</div>

<div class=""container"">

    <div class=""row"">
        <div class=""table table-striped"">
            <table>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Owned</th>
                        <th>Price</th>
                        <th>Rating</th>
                        <th>Ranking</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
");
            EndContext();
#line 29 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                         foreach (var item in Model.Dlc.Values)
                        {
                            var computedRanking = item.Owned ? "N/A" : item.Ranking.ToString();


#line default
#line hidden
            BeginContext(854, 62, true);
            WriteLiteral("                        <tr>\n\n                            <td>");
            EndContext();
            BeginContext(917, 7, false);
#line 35 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                           Write(item.Id);

#line default
#line hidden
            EndContext();
            BeginContext(924, 38, true);
            WriteLiteral("</td>\n                            <td>");
            EndContext();
            BeginContext(963, 9, false);
#line 36 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                           Write(item.Name);

#line default
#line hidden
            EndContext();
            BeginContext(972, 38, true);
            WriteLiteral("</td>\n                            <td>");
            EndContext();
            BeginContext(1011, 10, false);
#line 37 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                           Write(item.Owned);

#line default
#line hidden
            EndContext();
            BeginContext(1021, 38, true);
            WriteLiteral("</td>\n                            <td>");
            EndContext();
            BeginContext(1060, 10, false);
#line 38 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                           Write(item.Price);

#line default
#line hidden
            EndContext();
            BeginContext(1070, 38, true);
            WriteLiteral("</td>\n                            <td>");
            EndContext();
            BeginContext(1109, 11, false);
#line 39 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                           Write(item.Rating);

#line default
#line hidden
            EndContext();
            BeginContext(1120, 38, true);
            WriteLiteral("</td>\n                            <td>");
            EndContext();
            BeginContext(1159, 15, false);
#line 40 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                           Write(computedRanking);

#line default
#line hidden
            EndContext();
            BeginContext(1174, 104, true);
            WriteLiteral("</td>\n                            <td>\n                                <button class=\"btn btn-secondary\"");
            EndContext();
            BeginWriteAttribute("onclick", " onclick=\"", 1278, "\"", 1352, 3);
            WriteAttributeValue("", 1288, "location.href=\'", 1288, 15, true);
#line 42 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
WriteAttributeValue("", 1303, Url.Action("Edit", "Dlc", new { id = item.Id }), 1303, 48, false);

#line default
#line hidden
            WriteAttributeValue("", 1351, "\'", 1351, 1, true);
            EndWriteAttribute();
            BeginContext(1353, 106, true);
            WriteLiteral(" title=\"Edit\" value=\"Edit\">Edit</button>\n                            </td>\n\n                        </tr>\n");
            EndContext();
#line 46 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
                    }

#line default
#line hidden
            BeginContext(1481, 158, true);
            WriteLiteral("                    </tbody>\n                </table>\n            </div>\n        </div>\n\n        <div class=\"row\">\n            <button class=\"btn btn-primary\"");
            EndContext();
            BeginWriteAttribute("onclick", " onclick=\"", 1639, "\"", 1716, 3);
            WriteAttributeValue("", 1649, "location.href=\'", 1649, 15, true);
#line 53 "C:\Users\matt\Documents\GitHub\gamemanager\gamemanager\Views\Dlc\ViewDlc.cshtml"
WriteAttributeValue("", 1664, Url.Action("AddDlc", "Dlc", new { id = Model.Id }), 1664, 51, false);

#line default
#line hidden
            WriteAttributeValue("", 1715, "\'", 1715, 1, true);
            EndWriteAttribute();
            BeginContext(1717, 76, true);
            WriteLiteral(" title=\"AddDlc\" value=\"AddDlc\">Add Dlc</button>\n        </div>\n\n    </div>\n\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<GameEntry> Html { get; private set; }
    }
}
#pragma warning restore 1591
