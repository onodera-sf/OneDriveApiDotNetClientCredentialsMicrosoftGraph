using Azure.Identity;
using Microsoft.Graph;

// 各種 ID などの定義
var cliendId = "XXXXXXXX";      // クライアント ID
var tenantId = "XXXXXXXX";      // テナント ID
var clientSecret = "XXXXXXXX";  // クライアント シークレット
var userId = "XXXXXXXX";        // ユーザー ID

// 使いまわすので最初に定義しておく
using HttpClient httpClient = new();

// クライアント シークレットによる認証情報を定義
ClientSecretCredential clientSecretCredential = new(tenantId, cliendId, clientSecret);

// HttpClient と認証情報で Microsoft Graph サービスのクライアントを生成
using GraphServiceClient graphClient = new(httpClient, clientSecretCredential);

// 対象ユーザーに紐づく OneDrive を取得 (紐づいているドライブが OneDrive １つという前提)
var drive = await graphClient.Users[userId].Drive.GetAsync();
if (drive == null)
{
  Console.WriteLine("ドライブを取得できませんでした。");
  return;
}

// OneDrive のルートを取得
var root = await graphClient.Drives[drive.Id].Root.GetAsync();
if (root == null)
{
  Console.WriteLine("OneDrive のルートを取得できませんでした。");
  return;
}

// ルート直下にあるフォルダ一覧取得
var rootChildren = await graphClient.Drives[drive.Id].Items[root.Id].Children.GetAsync();
if (rootChildren == null || rootChildren.Value == null)
{
  Console.WriteLine("フォルダの一覧を取得できませんでした。");
  return;
}
foreach (var item in rootChildren.Value)
{
  Console.WriteLine($"Type={(item.File != null ? "File" : "Folder")}, Id={item.Id}, Name={item.Name}, Size={item.Size}");
}

