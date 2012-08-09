﻿using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using O2.DotNetWrappers.ExtensionMethods;
//using O2.DotNetWrappers.DotNet;

namespace O2.AutoGeneratedExe
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        public static bool saveEmbededLibrariesToDisk = true;
        public static string saveEmbededLibrariesToFolder = "_temp";
        
        static void Main(string[] args)
        {        	
            setAssemblyResolver();                        
            //loadDependencies();
            new Program().invokeMain(args);
            
        }

        public void invokeMain(string[] args)
        {            
        	//var assembly = getAssemblyfromResources("GraphSharp", true);
            new DynamicType().dynamicMethod();
        }

        public static void setAssemblyResolver()
        {
        	var startAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
        	saveEmbededLibrariesToFolder = Path.Combine(saveEmbededLibrariesToFolder, "_Files For " + startAssemblyName);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);			
        }
        /*public static void loadDependencies()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
			foreach(var name in entryAssembly.referencedAssemblies(true).names())
			{
				//if (name.contains(".tmp"))		
				{
					var location = AssemblyResolver.loadFromDiskOrResource(name).location().debug();
					//location.file_Copy(entryAssembly.Location.directoryName());
				}
			}
        }*/

        public static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var nameToFind = args.Name;
        	if (nameToFind.IndexOf(",") > -1)
                nameToFind = nameToFind.Substring(0,nameToFind.IndexOf(","));
            return getAssemblyfromResources(nameToFind, saveEmbededLibrariesToDisk);
     	}
     	
     	public static Assembly getAssemblyfromResources(string nameToFind, bool saveToDisk)
     	{     		
     		var nameToFind_Lower = nameToFind.ToLower();
            var targetAssembly = Assembly.GetExecutingAssembly();
            //var nameToFind = "O2_FluentSharp_CoreLib.dll";
            foreach (var resourceName in targetAssembly.GetManifestResourceNames())
                if (resourceName.ToLower().Contains(nameToFind_Lower) && 
                	(resourceName.ToLower().Contains(nameToFind_Lower + ".dll") || resourceName.ToLower().Contains(nameToFind_Lower+".exe")) )
                {
                    var assemblyStream = targetAssembly.GetManifestResourceStream(resourceName);
                    byte[] compressedData = new BinaryReader(assemblyStream).ReadBytes((int)assemblyStream.Length);
                    var data = gzip_Decompress(compressedData);                    
                    if (saveToDisk)
                    {
                    	var targetDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    	targetDir = Path.Combine(targetDir,saveEmbededLibrariesToFolder);
                    	if (Directory.Exists(targetDir) == false)
                    		Directory.CreateDirectory(targetDir);
                    	var targetFile = Path.Combine(targetDir, nameToFind + (resourceName.Contains(".dll") ? ".dll" : ".exe"));                    	
                    	if (File.Exists(targetFile) == false)
                    	{
	                    	using (FileStream fs = File.Create(targetFile))
	                		{
	                    		fs.Write(data, 0, data.Length);
	                    		fs.Close();                    
	                		}
	                	}
                		return Assembly.LoadFrom(targetFile);
                    }
                    return Assembly.Load(data);
                }
            return null;
     	}
     	
     	public static byte[] gzip_Decompress(byte[] bytes)
		{
			var inputStream = new MemoryStream();
			inputStream.Write(bytes, 0, bytes.Length);
			inputStream.Position = 0;
			var outputStream = new MemoryStream();
			using (var gzipStream= new GZipStream(inputStream,CompressionMode.Decompress))
			{
			    byte[] buffer = new byte[4096];
			    int numRead;
			    while ((numRead = gzipStream.Read(buffer, 0, buffer.Length)) != 0)			    
			        outputStream.Write(buffer, 0, numRead);			    
			}	  
			outputStream.Position=0;
			return outputStream.GetBuffer();
		}
            /*
            var name = args.prop("Name").str();
            var nameToFind = (name.isAssemblyName())
                    ? name.assemblyName().Name
                    : name.lower();

            foreach (var currentAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(currentAssembly.name().starts("O2"))
                    foreach (var resourceName in currentAssembly.GetManifestResourceNames())
                    {
                        if (resourceName.lower().contains(nameToFind.lower()))
                        {
                            "Found resource for {0} at {1} in {2}".info(name, resourceName, currentAssembly.name());
                            var assemblyStream = currentAssembly.GetManifestResourceStream(resourceName);
                            byte[] data = new BinaryReader(assemblyStream).ReadBytes((int)assemblyStream.Length);                            
                            return Assembly.Load(data);
                        }
                    }
            }
            return null;                    
        }*/

/*        private static void loadCoreLibFromEmbedded()
        {
            var assembly = getCoreLib();            
        }

        public static Assembly getCoreLib()
        {
            var targetAssembly =  Assembly.GetExecutingAssembly();
            var nameToFind = "O2_FluentSharp_CoreLib.dll";
            foreach (var resourceName in targetAssembly.GetManifestResourceNames())
                if (resourceName.Contains(nameToFind))
                {
                    var assemblyStream = targetAssembly.GetManifestResourceStream(resourceName);
                    byte[] data = new BinaryReader(assemblyStream).ReadBytes((int)assemblyStream.Length);
                    return Assembly.Load(data);
                }
            return null;
        }*/
        
    }
}
