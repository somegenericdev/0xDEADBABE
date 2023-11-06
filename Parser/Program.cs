using Parser;

var source=File.ReadAllText("/home/ubuntu/RiderProjects/Parser/Parser/source_code.0xb");

var scanner=new Scanner(source);
var res=scanner.Scan();
Console.Write("ciao");