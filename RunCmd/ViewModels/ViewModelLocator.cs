using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunCmd.ViewModels
{
   public class ViewModelLocator
    {
       public ViewModelLocator()
       {
           //Set up any dependency injection here
           
           //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
           
       }

       public HomeViewModel HomeView { get { return new HomeViewModel(); } }
       public OptionsWindowViewModel OptionsView { get { return new OptionsWindowViewModel(); } }
       public LogsViewModel LogsView { get { return new LogsViewModel(); } }
    }
}
