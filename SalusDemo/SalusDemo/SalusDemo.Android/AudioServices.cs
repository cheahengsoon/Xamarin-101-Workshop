using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;

using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SalusDemo.Droid;
using Android.Media;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AudioServices))]
namespace SalusDemo.Droid
{

    public class AudioServices : IAudio
    {
        public void PlayAudioFile(string fileName)
        {
            var player = new MediaPlayer();
            var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
            player.Prepared += (s, e) =>
            {
                player.Start();
            };
            player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
            player.Prepare();
        }
    }

  
}