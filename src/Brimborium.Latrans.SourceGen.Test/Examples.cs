using Microsoft.CodeAnalysis;

namespace Brimborium.Latrans.SourceGen {
    public class Examples {

        public static Compilation CreateReferencedLocationCompilation() {
            string source = @"
            namespace ReferencedAssembly
            {
                public class Location
                {
                    public int Id { get; set; }
                    public string Address1 { get; set; }
                    public string Address2 { get; set; }
                    public string City { get; set; }
                    public string State { get; set; }
                    public string PostalCode { get; set; }
                    public string Name { get; set; }
                    public string PhoneNumber { get; set; }
                    public string Country { get; set; }
                }
            }";

            return CompilationHelper.CreateCompilation(source);
        }

        public static Compilation CreateCampaignSummaryViewModelCompilation() {
            string source = @"
            namespace ReferencedAssembly
            {
                public class CampaignSummaryViewModel
                {
                    public int Id { get; set; }
                    public string Title { get; set; }
                    public string Description { get; set; }
                    public string ImageUrl { get; set; }
                    public string OrganizationName { get; set; }
                    public string Headline { get; set; }
                }
            }";

            return CompilationHelper.CreateCompilation(source);
        }

        public static Compilation CreateActiveOrUpcomingEventCompilation() {
            string source = @"
            using System;
            namespace ReferencedAssembly
            {
                public class ActiveOrUpcomingEvent
                {
                    public int Id { get; set; }
                    public string ImageUrl { get; set; }
                    public string Name { get; set; }
                    public string CampaignName { get; set; }
                    public string CampaignManagedOrganizerName { get; set; }
                    public string Description { get; set; }
                    public DateTimeOffset StartDate { get; set; }
                    public DateTimeOffset EndDate { get; set; }
                }
            }";

            return CompilationHelper.CreateCompilation(source);
        }

        public static Compilation CreateReferencedHighLowTempsCompilation() {
            string source = @"
            namespace ReferencedAssembly
            {
                public class HighLowTemps
                {
                    public int High { get; set; }
                    public int Low { get; set; }
                }
            }";

            return CompilationHelper.CreateCompilation(source);
        }

        public static Compilation CreateRepeatedLocationsCompilation() {
            string source = @"
            using System;
            using System.Collections;
            using System.Collections.Generic;
            using System.Text.Json.Serialization;

            [assembly: JsonSerializable(typeof(Fake.Location))]
            [assembly: JsonSerializable(typeof(HelloWorld.Location))]

            namespace Fake
            {
                public class Location
                {
                    public int FakeId { get; set; }
                    public string FakeAddress1 { get; set; }
                    public string FakeAddress2 { get; set; }
                    public string FakeCity { get; set; }
                    public string FakeState { get; set; }
                    public string FakePostalCode { get; set; }
                    public string FakeName { get; set; }
                    public string FakePhoneNumber { get; set; }
                    public string FakeCountry { get; set; }
                }
            }

            namespace HelloWorld
            {                
                public class Location
                {
                    public int Id { get; set; }
                    public string Address1 { get; set; }
                    public string Address2 { get; set; }
                    public string City { get; set; }
                    public string State { get; set; }
                    public string PostalCode { get; set; }
                    public string Name { get; set; }
                    public string PhoneNumber { get; set; }
                    public string Country { get; set; }
                }
            }";

            return CompilationHelper.CreateCompilation(source);
        }
    }
}
