﻿using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace RBAC
{
    class UpdatePoliciesFromYamlProgram
    {
        /// <summary>
        /// This method reads in the Yaml file with access policy changes and updates these policies in Azure.
        /// </summary>
        /// <param name="args">Contains the Json directory and Yaml directory</param>
        static void Main(string[] args)
        {
            Constants.toggle = "phase2";

            Console.WriteLine("Reading input file...");
            AccessPoliciesToYaml.verifyFileExtensions(args);
            JsonInput vaultList = AccessPoliciesToYaml.readJsonFile(args[0]);
            Console.WriteLine("Success!");

            Console.WriteLine("Grabbing secrets...");
            var secrets = AccessPoliciesToYaml.getSecrets(vaultList);
            Console.WriteLine("Success!");

            Console.WriteLine("Creating KeyVaultManagementClient and GraphServiceClient...");
            var kvmClient = AccessPoliciesToYaml.createKVMClient(secrets);
            var graphClient = AccessPoliciesToYaml.createGraphClient(secrets);
            Console.WriteLine("Success!");

            Console.WriteLine("Retrieving key vaults...");
            List<KeyVaultProperties> vaultsRetrieved = AccessPoliciesToYaml.getVaults(vaultList, kvmClient, graphClient);
            Console.WriteLine("Success!");

            Console.WriteLine("Reading yaml file...");
            List<KeyVaultProperties> yamlVaults = UpdatePoliciesFromYaml.deserializeYaml(args[1]);
            int changes = UpdatePoliciesFromYaml.checkChanges(yamlVaults, vaultsRetrieved);
            Console.WriteLine("Success!");
            if (changes != 0)
            {
                Console.WriteLine("Updating key vaults...");
                UpdatePoliciesFromYaml.updateVaults(yamlVaults, vaultsRetrieved, kvmClient, secrets, graphClient);
                Console.WriteLine("Updates finished!");
            }
            else
            {
                Console.WriteLine("There is no difference between the YAML and the Key Vaults. No changes made");
            }
        }
    }
}


