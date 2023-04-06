using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Arquivos
{
    //Files Upload to Api
    public static class Arquivos
    {
        public static void Send()
        {
            //Obtém fotos da pasta // Get pictures from the folder
            string dir = Directory.GetCurrentDirectory();
            string raiz = dir.Split("Software/")[0];
            string pictures = raiz + "Pictures/";
            string[] allfiles = Directory.GetFiles(pictures, "*.*", SearchOption.AllDirectories);

            //Define requisição / Defines request
            var client = new RestClient("[API BASE URL]/api/Upload/");
            var request = new RestRequest("[API BASE URL]/api/Upload/", Method.Post);
            
            //Inclui fotos na requisição //Includes pictures in the request
            for (int i = 0; i < allfiles.Count(); i++)
            {
                string filePath = allfiles[i];
                request.AddFile("someFile" + i.ToString(), filePath);
                Console.WriteLine(filePath);
            }

            //Chama api //Calls api
            Task<RestResponse> response = client.ExecuteAsync(request);
            response.Wait();

            //Apaga fotos //Delete pictures
            for (int i = 0; i < allfiles.Count(); i++)
            {
                string filePath = allfiles[i];
                File.Delete(filePath);
            }
        }
    }
}
