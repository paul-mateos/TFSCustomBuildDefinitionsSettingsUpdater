using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.IO;
using System.Configuration;
using Microsoft.TeamFoundation.Build.Activities.Extensions;
//using Microsoft.TeamFoundation.Lab.Workflow.Activities;
using Microsoft.TeamFoundation.Build.Client;



namespace Templates
{

   [BuildActivity(HostEnvironmentOption.Agent)]
    public sealed class UpdateAppSetting : CodeActivity
    {
       [RequiredArgument]
        public InArgument<string> FileName { get; set; }

       [RequiredArgument]
       public InArgument<string> Section { get; set; } 

        //[RequiredArgument]
        //public InArgument<string> Name { get; set; } 
 
        [RequiredArgument]
        public InArgument<string> envValue { get; set; }

        [RequiredArgument]
        public InArgument<string> browserValue { get; set; } 
 
        protected override void Execute(CodeActivityContext context)
        {
            
            string fileName = context.GetValue(FileName);
            string section = context.GetValue(Section);
            //string name = context.GetValue(Name); 
            string envvalue = context.GetValue(envValue);
            string browservalue = context.GetValue(browserValue);

            UpdateConfigFileAppSetting(fileName, section, "Environment", envvalue);
            UpdateConfigFileAppSetting(fileName, section, "Browser", browservalue);
            
           


        } 
 
        /// <summary>
        /// Update an app setting in an app config file. If it is not present then add it.
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void UpdateConfigFileAppSetting(string configFile, string section, string name, string value)
        {
            MakeWritable(configFile);
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ClientSettingsSection applicationSettingsSection = (ClientSettingsSection)config.SectionGroups["applicationSettings"].Sections[section];
            SettingElement element = applicationSettingsSection.Settings.Get(name);

            if (null != element)
            {
                applicationSettingsSection.Settings.Remove(element);
                element.Value.ValueXml.InnerXml = value;
                applicationSettingsSection.Settings.Add(element);
            }
            else
            {
                element = new SettingElement(name, SettingsSerializeAs.String);
                element.Value = new SettingValueElement();
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                element.Value.ValueXml = doc.CreateElement("value");

                element.Value.ValueXml.InnerXml = value;
                applicationSettingsSection.Settings.Add(element);
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("applicationSettings");

        } 
 
        /// <summary>
        /// Make a file writeable
        /// </summary>
        /// <param name="file"></param>
        private void MakeWritable(string file)
        {
            //string fileLocation = System.Environment.GetEnvironmentVariable("TF_BUILD_BINARIESDIRECTORY");
            FileInfo fileInfo = new System.IO.FileInfo(file);
            if (fileInfo.IsReadOnly)
                fileInfo.IsReadOnly = false;
        }

     
    } 
}
