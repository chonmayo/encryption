using System;

//using e = encryption;
class Final_Proj{
    public static void Main(){
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("[R]Rot13 \nPick your encryption: ");
        char encryption = char.ToLower(char.Parse(Console.ReadLine()));
        switch (encryption){
            case 'r':
                Rot13 r = new Rot13();
                Console.WriteLine(r.go());
                break;
            default:
                break;
        }
        
        
        
    }
}
class Rot13{
    public string go(){
        Console.Write("Enter your string: ");
        string plaintext = Console.ReadLine();
        string encrypted_text="";
        foreach(byte b in System.Text.Encoding.UTF8.GetBytes(plaintext.ToCharArray())){
                if(b>122||b<65){continue;}
                int c = (int)Char.ToUpper((char)b);
                encrypted_text+=(char)('A'+(c-(int)'A'+13) % 26);}
        return encrypted_text;
    }
}
