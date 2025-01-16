using System.Collections.Concurrent;

namespace ServerSideEvents.API;

public class ItemService
{
	private static readonly ConcurrentQueue<Message> _messageQueue = new();

	public void Reset() {
		_messageQueue.Clear();
	}

	public void AddItemAvailable(Message message) {
		_messageQueue.Enqueue(message);
		Console.WriteLine($"Added new item to the queue, total in queue {_messageQueue.Count}");
	}

	public async Task<Message?> WaitForNewItem() {
		Message? message = null;
		while(message == null) {
			if(!_messageQueue.TryDequeue(out message)) {
				await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1,10)));
			}
		}
		return message;
	}
}