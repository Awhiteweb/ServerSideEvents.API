using System.Collections.Concurrent;

namespace ServerSideEvents.API;

public class ItemService
{
	private static readonly ConcurrentQueue<Message> _messageQueue = new();
	private static readonly ConcurrentQueue<Message> _adminQueue = new();
	private static readonly ConcurrentQueue<Message> _userQueue = new();

	public void Reset() {
		_messageQueue.Clear();
	}

	public void AddItemAvailable(Message message) {
		if(message.Item.Value < 250) {
			_adminQueue.Enqueue(message);
			Console.WriteLine($"Added new item to admin queue, total in queue {_adminQueue.Count}");
		}
		else {
			_userQueue.Enqueue(message);
			Console.WriteLine($"Added new item to user queue, total in queue {_userQueue.Count}");
		}
		_messageQueue.Enqueue(message);
		Console.WriteLine($"Added new item to all queue, total in queue {_messageQueue.Count}");
	}

	public async Task<Message?> WaitForNewItem(string? token) {
		Message? message = null;
		while(message == null) {
			if(token == "all") {
				Console.WriteLine("Pulling from all queue");
				if(!_messageQueue.TryDequeue(out message)) {
					await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1,10)));
				}
			}
			else if(token == "admin") {
				Console.WriteLine("Pulling from admin queue");
				if(!_adminQueue.TryDequeue(out message)) {
					await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1,10)));
				}
			}
			else {
				Console.WriteLine("Pulling from user queue");
				if(!_userQueue.TryDequeue(out message)) {
					await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1,10)));
				}
			}
		}
		return message;
	}
}