fetch('./imlang.wasm').then(
	response => response.arrayBuffer()).then(
	bytes => WebAssembly.instantiate(bytes)).then(
	results => {run(results.instance)}).catch(console.error);

function run(wasm)
{
	var container = document.getElementById("container");
	var list = wasm.exports.listNew(123);
	wasm.exports.listAdd(list, 10);
	wasm.exports.listAdd(list, -20);

		container.textContent += wasm.exports.listSum(list);
		container.textContent += "\r\n";
		
	const memory = new Uint32Array(wasm.exports.mem.buffer, 0, 1024);
	container.textContent += memory;
}