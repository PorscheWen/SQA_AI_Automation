using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Demo2Desktop.Plugin
{
    public sealed class PluginManager
    {
        readonly List<IToolbarPlugin> plugins = new List<IToolbarPlugin>();

        public IList<IToolbarPlugin> Plugins
        {
            get { return plugins.AsReadOnly(); }
        }

        public void LoadAll(string pluginsDirectory)
        {
            plugins.Clear();
            plugins.Add(new BuiltInHelloPlugin());

            if (!Directory.Exists(pluginsDirectory))
                return;

            string[] files = Directory.GetFiles(pluginsDirectory, "*.dll");
            foreach (string file in files)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(file);
                    foreach (Type type in asm.GetTypes())
                    {
                        if (type.IsAbstract || type.IsInterface)
                            continue;
                        if (!typeof(IToolbarPlugin).IsAssignableFrom(type))
                            continue;

                        IToolbarPlugin instance = (IToolbarPlugin)Activator.CreateInstance(type);
                        if (instance != null)
                            plugins.Add(instance);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "無法載入外掛: " + Path.GetFileName(file) + "\n" + ex.Message,
                        "Plugin",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }
    }
}
