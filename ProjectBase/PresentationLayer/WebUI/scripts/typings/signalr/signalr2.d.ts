interface IPromise<T>
{
	always(...alwaysCallbacks: any[]): IPromise<T>;
	done(...doneCallbacks: any[]): IPromise<T>;
	fail(...failCallbacks: any[]): IPromise<T>;
	progress(...progressCallbacks: any[]): IPromise<T>;
	then<U>(onFulfill: (...values: any[]) => U, onReject?: (...reasons: any[]) => U, onProgress?: (...progression: any[]) => any): IPromise<U>;
}
interface IDeferred<T> extends IPromise<T>
{
	always(...alwaysCallbacks: any[]): IDeferred<T>;
	done(...doneCallbacks: any[]): IDeferred<T>;
	fail(...failCallbacks: any[]): IDeferred<T>;
	progress(...progressCallbacks: any[]): IDeferred<T>;
	notify(...args: any[]): IDeferred<T>;
	notifyWith(context: any, ...args: any[]): IDeferred<T>;
	reject(...args: any[]): IDeferred<T>;
	rejectWith(context: any, ...args: any[]): IDeferred<T>;
	resolve(val: T): IDeferred<T>;
	resolve(...args: any[]): IDeferred<T>;
	resolveWith(context: any, ...args: any[]): IDeferred<T>;
	state(): string;
	promise(target?: any): IPromise<T>;
}
interface SignalR
{
	ajaxDefaults: any;
}