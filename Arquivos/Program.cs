using RestSharp;

//Testing program - Not used in the solution

Console.WriteLine("Hello, World!");
Console.ReadLine();
string dir = Directory.GetCurrentDirectory();
string raiz = dir.Split("Software/")[0];
string pictures = raiz + "Pictures/";
string[] allfiles = Directory.GetFiles(pictures, "*.*", SearchOption.AllDirectories); 

var client = new RestClient("[API BASE URL]/api/Upload/");
var request = new RestRequest("[API BASE URL]/api/Upload/", Method.Post);

for (int i = 0; i < allfiles.Count(); i++)
{
    string filePath = allfiles[i];
    request.AddFile("someFile" + i.ToString(), filePath);
    Console.WriteLine(filePath);
}

Task<RestResponse> response = client.ExecuteAsync(request);
response.Wait();
Console.WriteLine(response.Result.Content);
for (int i = 0; i < allfiles.Count(); i++)
{
    string filePath = allfiles[i];
    Console.WriteLine("Deleting..." + filePath);
    File.Delete(filePath);
}
Console.ReadLine();
