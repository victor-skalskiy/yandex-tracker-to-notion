namespace YandexTrackerToNotion.Interfaces
{
	public interface IHelperService
	{
		string GetRequestFullBody(HttpRequest request, string postData);
	}
}