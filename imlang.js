fetch('./imlang.wasm').then(
	response => response.arrayBuffer()).then(
	bytes => WebAssembly.instantiate(bytes)).then(
	results => {run(results.instance)}).catch(console.error);

function run(wasm)
{
	var container = document.getElementById("container");

	for(var i=0; i<10; i++)
	{
		var a = Math.floor(Math.random()*100);
		var b = Math.floor(Math.random()*100);

		container.textContent += a + " + " + b + " = ";
		container.textContent += wasm.exports.add(a, b);
		container.textContent += "\r\n";
	}
}