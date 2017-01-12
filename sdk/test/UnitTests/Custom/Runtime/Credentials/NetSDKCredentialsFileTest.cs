/*
 * Copyright 2016 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Settings;
using Amazon.Util;
using AWSSDK_DotNet.IntegrationTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AWSSDK.UnitTests
{
    [TestClass]
    public class NetSDKCredentialsFileTest
    {
        private const string ReservedFieldExceptionFormat = "The profile properties cannot contain reserved names as keys: {0}";
        private const string CredentialsFieldExceptionFormat = "The profile properties dictionary cannot contain a key named {0} because it is in the name mapping dictionary.";

        private const string AWSCredentialsProfileType = "AWS";
        private const string SAMLRoleProfileType = "SAML";
        private const string UniqueKey = "deefa421-989c-4dd6-9dbc-baecfb5e63f6";

        private static readonly string InvalidProfileText = new StringBuilder()
            .AppendLine("{")
            .AppendLine("    \"" + UniqueKey + "\" : {")
            .AppendLine("        \"DisplayName\" : \"InvalidProfile\",")
            .AppendLine("        \"SessionType\" : \"AWS\",")
            .AppendLine("        \"AWSAccessKey\" : \"access_key_id\",")
            .AppendLine("    }")
            .AppendLine("}").ToString();

        private static readonly string LegacyCredentialsTypeProfileText = new StringBuilder()
            .AppendLine("{")
            .AppendLine("    \"" + UniqueKey + "\" : {")
            .AppendLine("        \"DisplayName\" : \"LegacyCredentialsTypeProfile\",")
            .AppendLine("        \"CredentialsType\" : \"the_credentials_type\",")
            .AppendLine("    }")
            .AppendLine("}").ToString();

        [TestMethod]
        public void TestAllProfileTypes()
        {
            foreach (CredentialProfileType type in Enum.GetValues(typeof(CredentialProfileType)))
            {
                using (var tester = new NetSDKCredentialsFileTestFixture())
                {
                    var profileName = type.ToString() + Guid.NewGuid().ToString();
                    var originalProfile = CredentialProfileTestHelper.GetRandomProfile(profileName, type, tester.ProfileStore);
                    Assert.IsTrue(originalProfile.CanCreateAWSCredentials);
                    Assert.IsNull(originalProfile.UniqueKey);

                    tester.ProfileStore.RegisterProfile(originalProfile);

                    //make sure the store assigns a unique key
                    Assert.IsNotNull(originalProfile.UniqueKey);
                    var readProfile = tester.TestTryGetProfile(profileName, true, true);
                    Assert.AreEqual(originalProfile, readProfile);

                    // make sure the ProfileType is written, even though it's ignored
                    var expectedType = type.ToString();
                    if (type == CredentialProfileType.Basic)
                    {
                        expectedType = AWSCredentialsProfileType;
                    } else if (type == CredentialProfileType.SAMLRole ||
                        type == CredentialProfileType.SAMLRoleUserIdentity)
                    {
                        expectedType = SAMLRoleProfileType;
                    }
                    tester.AssertJsonProperty(profileName, SettingsConstants.ProfileTypeField, expectedType);
                }
            }
        }

        [TestMethod]
        public void RegisterProfileReservedPropertyName1()
        {
            TestReservedPropertyName(ReservedFieldExceptionFormat, "DisplayName");
        }

        [TestMethod]
        public void RegisterProfileReservedPropertyName2()
        {
            TestReservedPropertyName(ReservedFieldExceptionFormat, "ProfileType");
        }

        [TestMethod]
        public void RegisterProfileReservedPropertyName3()
        {
            TestReservedPropertyName(CredentialsFieldExceptionFormat, "RoleArn");
        }

        [TestMethod]
        public void RegisterProfileReservedPropertyName4()
        {
            TestReservedPropertyName(ReservedFieldExceptionFormat, "Region");
        }

        [TestMethod]
        public void RegisterProfileReservedPropertyNameIgnoreCase()
        {
            TestReservedPropertyName(CredentialsFieldExceptionFormat, "rolearn");
        }

        private void TestReservedPropertyName(string exceptionFormat, string propertyName)
        {
            AssertExtensions.ExpectException(() =>
            {
                using (var tester = new NetSDKCredentialsFileTestFixture())
                {
                    var profileName = Guid.NewGuid().ToString();
                    var profile = CredentialProfileTestHelper.GetRandomProfile(profileName, CredentialProfileType.Basic, tester.ProfileStore);
                    var properties = CredentialProfileUtils.GetProperties(profile);
                    properties.Add(propertyName, "aargh!");
                    tester.ProfileStore.RegisterProfile(profile);
                }
            }, typeof(ArgumentException), string.Format(CultureInfo.InvariantCulture, exceptionFormat, propertyName));
        }

        [TestMethod]
        public void ProfileNotFound()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                tester.TestTryGetProfile("DoesNotExist", false, false);
            }
        }

        [TestMethod]
        public void InvalidProfile()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture(InvalidProfileText))
            {
                tester.TestTryGetProfile("InvalidProfile", true, false);
            }
        }

        [TestMethod]
        public void LegacyCredentialsTypeProfile()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture(LegacyCredentialsTypeProfileText))
            {
                var profile = tester.TestTryGetProfile("LegacyCredentialsTypeProfile", true, false);
                var credentialType = CredentialProfileUtils.GetProperty(profile, "CredentialsType");
                Assert.AreEqual("the_credentials_type", credentialType);
            }
        }

        [TestMethod]
        public void ReadUniqueKeyProperty()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture(LegacyCredentialsTypeProfileText))
            {
                var profile = tester.TestTryGetProfile("LegacyCredentialsTypeProfile", true, false);
                Assert.AreEqual(UniqueKey, profile.UniqueKey);
            }
        }

        [TestMethod]
        public void UnregisterProfile()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                // register
                tester.ProfileStore.RegisterProfile(CredentialProfileTestHelper.GetCredentialProfile(CredentialProfileType.Basic.ToString(),
                    CredentialProfileTestHelper.GetRandomOptions(CredentialProfileType.Basic)));
                // check that it's there
                tester.TestTryGetProfile(CredentialProfileType.Basic.ToString(), true, true);

                // unregister
                tester.ProfileStore.UnregisterProfile(CredentialProfileType.Basic.ToString());
                // check that it's not there
                tester.TestTryGetProfile(CredentialProfileType.Basic.ToString(), false, false);

            }
        }


        [TestMethod]
        public void ListProfileNamesEmpty()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                var profileNames = tester.ProfileStore.ListProfileNames();
                Assert.AreEqual(0, profileNames.Count);
            }
        }

        [TestMethod]
        public void ListProfileNames()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                tester.ProfileStore.RegisterProfile(CredentialProfileTestHelper.GetCredentialProfile(
                       "SessionProfile", CredentialProfileTestHelper.GetRandomOptions(CredentialProfileType.Session)));

                var profileNames = tester.ProfileStore.ListProfileNames();
                Assert.AreEqual(1, profileNames.Count);
                Assert.IsTrue(profileNames.Contains("SessionProfile"));
            }
        }

        [TestMethod]
        public void ListProfileNamesExcludeInvalid()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture(InvalidProfileText))
            {
                tester.ProfileStore.RegisterProfile(CredentialProfileTestHelper.GetCredentialProfile(
                       "SessionProfile", CredentialProfileTestHelper.GetRandomOptions(CredentialProfileType.Session)));

                var profileNames = tester.ProfileStore.ListProfileNames();
                Assert.AreEqual(1, profileNames.Count);
                Assert.IsTrue(profileNames.Contains("SessionProfile"));
            }
        }

        [TestMethod]
        public void ListProfilesEmpty()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                var profiles = tester.ProfileStore.ListProfiles();
                Assert.AreEqual(0, profiles.Count);
            }
        }

        [TestMethod]
        public void ListProfiles()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                tester.ProfileStore.RegisterProfile(CredentialProfileTestHelper.GetCredentialProfile(
                       "SessionProfile", CredentialProfileTestHelper.GetRandomOptions(CredentialProfileType.Session)));

                var profiles = tester.ProfileStore.ListProfiles();
                Assert.AreEqual(1, profiles.Count);
                Assert.AreEqual("SessionProfile", profiles[0].Name);
            }
        }

        [TestMethod]
        public void ListProfilesExcludeInvalid()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture(InvalidProfileText))
            {
                tester.ProfileStore.RegisterProfile(CredentialProfileTestHelper.GetCredentialProfile(
                    "SessionProfile", CredentialProfileTestHelper.GetRandomOptions(CredentialProfileType.Session)));

                var profiles = tester.ProfileStore.ListProfiles();
                Assert.AreEqual(1, profiles.Count);
                Assert.AreEqual("SessionProfile", profiles[0].Name);
            }
        }

        [TestMethod]
        public void TestReadCompatibilityBasic()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                // write with old ProfileManager
                ProfileManager.RegisterProfile("BasicProfile", "AccessKey", "SecretKey");

                // read with new CredentialProfileManager
                CredentialProfile profile;
                Assert.IsTrue(tester.ProfileStore.TryGetProfile("BasicProfile", out profile));
                Assert.IsNotNull(profile);
                Assert.AreEqual("BasicProfile", profile.Name);
                Assert.AreEqual("AccessKey", profile.Options.AccessKey);
                Assert.AreEqual("SecretKey", profile.Options.SecretKey);
                Assert.AreEqual(CredentialProfileType.Basic, CredentialProfileTypeDetector.DetectProfileType(profile.Options).Value);
            }
        }

        [TestMethod]
        public void TestWriteCompatibilityBasic()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                // write with new CredentialProfileManager
                CredentialProfile profile = CredentialProfileTestHelper.GetCredentialProfile(
                       "BasicProfile", CredentialProfileTestHelper.GetRandomOptions(CredentialProfileType.Basic));
                tester.ProfileStore.RegisterProfile(profile);

                // read with old ProfileManager
                AWSCredentials credentials;
                Assert.IsTrue(ProfileManager.TryGetAWSCredentials("BasicProfile", out credentials));
                Assert.IsNotNull(credentials);
                var immutableCredentials = credentials.GetCredentials();
                Assert.AreEqual(profile.Options.AccessKey, immutableCredentials.AccessKey);
                Assert.AreEqual(profile.Options.SecretKey, immutableCredentials.SecretKey);
            }
        }

        [TestMethod]
        public void TestReadCompatibilitySAML()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                // write with old ProfileManager
                ProfileManager.RegisterSAMLEndpoint("EndpointName", new Uri("https://somesamlendpoint/"), null);
                ProfileManager.RegisterSAMLRoleProfile("SAMLProfile", "EndpointName", "RoleArn", "UserIdentity");

                // read with new CredentialProfileManager
                CredentialProfile profile;
                Assert.IsTrue(tester.ProfileStore.TryGetProfile("SAMLProfile", out profile));
                Assert.IsNotNull(profile);
                Assert.AreEqual("SAMLProfile", profile.Name);
                Assert.AreEqual("EndpointName", profile.Options.EndpointName);
                Assert.AreEqual("RoleArn", profile.Options.RoleArn);
                Assert.AreEqual("UserIdentity", profile.Options.UserIdentity);
                Assert.AreEqual(CredentialProfileType.SAMLRoleUserIdentity, CredentialProfileTypeDetector.DetectProfileType(profile.Options).Value);
            }
        }

        [TestMethod]
        public void TestWriteCompatibilitySAML()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                // write with new CredentialProfileManager
                CredentialProfile profile = CredentialProfileTestHelper.GetCredentialProfile(
                       "SAMLProfile", CredentialProfileTestHelper.GetRandomOptions(CredentialProfileType.SAMLRoleUserIdentity));
                tester.ProfileStore.RegisterProfile(profile);

                // TODO do this with the new SAML Endpoint Manager
                ProfileManager.RegisterSAMLEndpoint(profile.Options.EndpointName, new Uri("https://somesamlendpoint/"), null);

                // read with old ProfileManager
                SAMLRoleProfile samlProfile;
                Assert.IsTrue(ProfileManager.TryGetProfile("SAMLProfile", out samlProfile));
                Assert.IsNotNull(samlProfile);
                Assert.AreEqual(profile.Options.EndpointName, samlProfile.EndpointSettings.Name);
                Assert.AreEqual(profile.Options.RoleArn, samlProfile.RoleArn);
                Assert.AreEqual(profile.Options.UserIdentity, samlProfile.UserIdentity);
            }
        }

        [TestMethod]
        public void TestWriteCompatibilitySession()
        {
            using (var tester = new NetSDKCredentialsFileTestFixture())
            {
                // write a type that's not supported by ProfileManager
                tester.ProfileStore.RegisterProfile(CredentialProfileTestHelper.GetCredentialProfile(
                    "SessionProfile", CredentialProfileTestHelper.GetRandomOptions(CredentialProfileType.Session)));

                // make sure profile manager can't read it as a basic profile, and that there aren't any errors.
                AWSCredentials credentials;
                Assert.IsFalse(ProfileManager.TryGetAWSCredentials("SessionProfile", out credentials));
                Assert.IsNull(credentials);

                // make sure profile manager can't read it as a SAML profile, and that there aren't any errors.
                SAMLRoleProfile samlProfile;
                Assert.IsFalse(ProfileManager.TryGetProfile("SessionProfile", out samlProfile));
                Assert.IsNull(samlProfile);
            }
        }
    }
}