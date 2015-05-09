﻿
using Orchard.UI.Resources;

namespace ivNet.Listing
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("jQuery.Validate").SetUrl("jquery.validate.min.js").SetVersion("1.0").SetDependencies("jQueryUI");
            manifest.DefineScript("jQuery.Validate.Unobtrusive").SetUrl("jquery.validate.unobtrusive.min.js").SetVersion("1.0").SetDependencies("jQuery.Validate");

            manifest.DefineScript("jQuery.FileUpload").SetUrl("jquery.fileupload.js").SetVersion("5.42.3").SetDependencies("jQueryUI");

            manifest.DefineScript("AngularJS").SetUrl("ivNet/anjularJs/anjular.min.js").SetVersion("1.2.9").SetDependencies("jQueryUI");
            manifest.DefineScript("AngularJSResource").SetUrl("ivNet/anjularJs/angular-resource.min.js").SetVersion("1.2.9").SetDependencies("AngularJS");           
            manifest.DefineScript("trNgGrid").SetUrl("ivNet/trNgGrid/trNgGrid.min.js").SetVersion("1.2.9").SetDependencies("AngularJSResource");

            manifest.DefineScript("Bootstrap.TagsInput.Anjular").SetUrl("ivNet/anjularJs/bootstrap-tagsinput-angular.min.js").SetDependencies("AngularJS");           
            manifest.DefineScript("Bootstrap.TagsInput").SetUrl("ivNet/anjularJs/bootstrap-tagsinput.min.js").SetDependencies("Bootstrap.TagsInput.Anjular");

            manifest.DefineScript("CKEditor").SetUrl("ivNet/ckeditor/ckeditor.js").SetDependencies("jQueryUI");

            manifest.DefineScript("ivNet.Registration").SetUrl("ivNet/registration.min.js").SetVersion("1.0").SetDependencies("jQuery.Validate.Unobtrusive");

            manifest.DefineScript("ivNet.Listings")
                .SetUrl("ivNet/listings.min.js")
                .SetVersion("1.0")
                .SetDependencies("trNgGrid");

            manifest.DefineScript("ivNet.Configuration")
                .SetUrl("ivNet/configuration.min.js")
                .SetVersion("1.0")
                .SetDependencies("trNgGrid");

            manifest.DefineStyle("Font-Awesome").SetUrl("ivNet/Styles/font-awesome.min.css");
            manifest.DefineStyle("Bootstrap").SetUrl("ivNet/Styles/bootstrap.min.css").SetDependencies("Font-Awesome");
            manifest.DefineStyle("jQuery.FileUpload").SetUrl("ivNet/Styles/jquery.fileupload.css");
            manifest.DefineStyle("Bootstrap.TagsInput").SetUrl("ivNet/Styles/bootstrap-tagsinput.css").SetDependencies("Bootstrap");
            manifest.DefineStyle("ivNet.Listing").SetUrl("ivNet/Styles/ivNet.Listing.min.css");
            manifest.DefineStyle("ivNet.Configuration").SetUrl("ivNet/Styles/ivNet.Configuration.min.css");
        }
    }
}