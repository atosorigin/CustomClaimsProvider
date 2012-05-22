using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Diagnostics;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Claims;
using Microsoft.SharePoint.WebControls;


namespace SPCustomClaimsProvider
{
    public class CustomClaims : SPClaimProvider
    {

        public CustomClaims(string displayName)
            : base(displayName)
        {
        }

        public override bool SupportsEntityInformation
        {
            get
            {
                return false;
            }
        }

        public override bool SupportsHierarchy
        {
            get
            {
                return false;
            }
        }

        public override bool SupportsResolve
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsSearch
        {
            get
            {
                return true;
            }
        }

        internal static string ProviderDisplayName
        {
            get
            {
                return "Active Directory";
            }
        }


        internal static string ProviderInternalName
        {
            get
            {
                return "ActiveDirectoryProvider";
            }
        }


        public override string Name
        {
            get
            {
                return ProviderInternalName;
            }
        }

        private static string CustomClaimsType
        {
            get { return "http://schemas.pearson.local/PeoplePicker"; }
        }

        private static string CustomClaimsValueType
        {
            get
            {
                return Microsoft.IdentityModel.Claims.ClaimValueTypes.String;
            }
        }


        protected override void FillSearch(Uri context, string[] entityTypes,string searchPattern, string hierarchyNodeID, int maxCount,SPProviderHierarchyTree searchTree)
        {

            // Ensure that People Picker is asking for the type of entity that we 
            // return; site collection administrator will not return, for example.
            if (!EntityTypesContain(entityTypes, SPClaimEntityTypes.FormsRole))
                return;

            // The counter to track what node we are in; it will be used to call into
            // our helper arrays that were covered in part 1 and part 2 of this article.
            int teamNode = -1;

            SPProviderHierarchyNode matchNode = null;
            //Query ActiveDirectory for a user or a group with a name like the search string
            var adTools = new ActiveDirectory();
            var users = adTools.FindUsers(searchPattern);

            foreach (var user in users)
            {
                matchNode.AddEntity(GetPickerEntity(user));
            }


        }

        private PickerEntity GetPickerEntity(string user)
        {
            PickerEntity pe = CreatePickerEntity();

            // Set the claim that is associated with this match.
            pe.Claim = CreateClaim(CustomClaimsType, user, CustomClaimsValueType);

            // Set the tooltip that is displayed when you pause over the resolved claim.
            pe.Description = CustomClaims.ProviderDisplayName + ":" + user;

            // Set the text that we will display.
            pe.DisplayText = user;

            // Store it here, in the hashtable **
            pe.EntityData[PeopleEditorEntityDataKeys.DisplayName] = user;

            // We plug this in as a role type entity.
            pe.EntityType = SPClaimEntityTypes.FormsRole;

            // Flag the entry as being resolved.
            pe.IsResolved = true;

            return pe;
        }

        protected override void FillResolve(Uri context, string[] entityTypes, SPClaim resolveInput, List<PickerEntity> resolved)
        {
            // Ensure that People Picker is asking for the type of entity that we 
            // return; site collection administrator will not return, for example.
            if (!EntityTypesContain(entityTypes, SPClaimEntityTypes.FormsRole))
            return;

            var adTools = new ActiveDirectory();
            var users = adTools.FindUsers(resolveInput.Value.ToLower());

            foreach (var user in users)
            {
                resolved.Add(GetPickerEntity(user));
            }
        }

        protected override void FillHierarchy(Uri context, string[] entityTypes, string hierarchyNodeID, int numberOfLevels, SPProviderHierarchyTree hierarchy)
        {
            throw new NotImplementedException();
        }

        protected override void FillResolve(Uri context, string[] entityTypes, string resolveInput, List<PickerEntity> resolved)
        {
            if (!EntityTypesContain(entityTypes, SPClaimEntityTypes.FormsRole))
                return;

            var adTools = new ActiveDirectory();
            var users = adTools.FindUsers(resolveInput.ToLower());

            foreach (var user in users)
            {
                resolved.Add(GetPickerEntity(user));
            }
        }

        protected override void FillSchema(SPProviderSchema schema)
        {
            // Add the schema element that we need at a minimum in our picker node.
            schema.AddSchemaElement(new
                  SPSchemaElement(PeopleEditorEntityDataKeys.DisplayName,
                  "Display Name", SPSchemaElementType.Both));
        }

        protected override void FillClaimTypes(List<string> claimTypes)
        {
            throw new NotImplementedException();
        }

        protected override void FillClaimValueTypes(List<string> claimValueTypes)
        {
            throw new NotImplementedException();
        }

        protected override void FillEntityTypes(List<string> entityTypes)
        {
            // Return the type of entity claim that we are using (only one in this case).
            entityTypes.Add(SPClaimEntityTypes.FormsRole);
        }

        protected override void FillClaimsForEntity(Uri context, SPClaim entity, List<SPClaim> claims)
        {
            throw new NotImplementedException();
        }
    }
}
