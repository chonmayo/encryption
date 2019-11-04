
//how to calculate n. 
using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;

class encryption{
    public static void Main(){
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[1]Rot13\t[2]RSA \nPick your encryption: ");
        char encryption = char.ToLower(char.Parse(Console.ReadLine()));
        
        switch (encryption){
            case '1':
                Rot13 r = new Rot13();
                Console.WriteLine(r.go("plaintext"));
                break;
            case '2':
                RSA rSA = new RSA();
                rSA.go();
                break;
            default:
                break;
        }
    }
}
class Rot13{
    public string go(string plaintext){
        string encrypted_text="";
        foreach(byte b in System.Text.Encoding.UTF8.GetBytes(plaintext.ToCharArray())){
                if(b>122||b<65){encrypted_text+=(char)b;continue;}
                int c = (int)Char.ToUpper((char)b);
                encrypted_text+=(char)('A'+(c-(int)'A'+13) % 26);}
        return encrypted_text;
    }
}
/*
reference : https://www.di-mgt.com.au/rsa_alg.html#x931
*/
class RSA{
    public string go(){
        
        FileHelper fh = new FileHelper();

        Console.WriteLine("Generating keys...");
        keyGen();

        Console.Write("Enter plaintext file name: ");
        string filename = Console.ReadLine();

        Console.Write("Enter public key (int, int): ");
        string public_key = Console.ReadLine();
        int e  = int.Parse(public_key.Remove(0,public_key.IndexOf(',')+1).Trim());
        BigInteger n  = BigInteger.Parse(public_key.Remove(public_key.IndexOf(',')).Trim());
        Console.WriteLine("Encrypting file "+filename+"...");

        BigInteger enc = (encrypt(fh.FileInput(filename),e,n));
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Encrypted text: " + enc);
        Console.ForegroundColor = ConsoleColor.Green;
        
        fh.FileOutput(enc, "output.txt");

        Console.Write("Enter encrypted file name: ");
        filename = Console.ReadLine();

        Console.Write("Enter private key (int, int): ");
        string private_key = Console.ReadLine();
        BigInteger d  = BigInteger.Parse(private_key.Remove(0,private_key.IndexOf(',')+1).Trim());
        n  = BigInteger.Parse(private_key.Remove(private_key.IndexOf(',')).Trim());

        Console.WriteLine("Decrypting...");
        string dec = decrypt(BigInteger.Parse(fh.FileInput(filename)),d,n,3);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Decrypted text: " + dec);
        Console.ForegroundColor = ConsoleColor.Green;
        return "";
    }
    public void keyGen(){
        const int e = 7;
        BigInteger prime=65027;
        BigInteger qrime=65119;
        
        BigInteger n = qrime*prime;
        BigInteger t = (qrime-1)*(prime-1);
        //check that gcd of e and prime numbers is 1
        BigInteger e_t_gcd = gcd(e,t);

        List<BigInteger> hist = new List<BigInteger>(); 
        BigInteger d=1;
        if(e_t_gcd==1){
            d = ExtendedEuclidian(t,t,e,0,0,hist);
            }
        else{
                Console.WriteLine("e shares a factor with a prime: {0}",e_t_gcd);
            }
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(String.Format("Public Key: {0},{1}",n,e));
        Console.WriteLine(String.Format("Private Key: {0},{1}",n,d));
        Console.WriteLine(String.Format("Phi: {0} d: {1}",t,d));
        Console.ForegroundColor = ConsoleColor.Green;
    }
    public BigInteger encrypt(string plaintext, int e, BigInteger n){
        //encrypt plaintext,c,e,n
        int counter = plaintext.Length;
        BigInteger base_27 = 0;
       foreach(char b in System.Text.Encoding.UTF8.GetBytes(plaintext.ToCharArray())){
            counter--;
            BigInteger c = (b-64)*BigInteger.Pow(27,counter);
            base_27 += c;
       }
       BigInteger encrypted = BigInteger.Pow(base_27,e);
       return encrypted;
    }
    public string decrypt(BigInteger encrypted_text, BigInteger d, BigInteger n, int l){
        int encrypted_length = l;
        string decrypted_text="";
        BigInteger base_27 = mod_exp(encrypted_text,d,n);
        //int counter = 0;
        int t = 1;
        while(t!=0 && encrypted_length>0){
            encrypted_length--;
            BigInteger divisor = BigInteger.Pow(27,encrypted_length);
            decrypted_text+=(char)((base_27/divisor)+64);
            base_27 = base_27%divisor;

        }
        return (decrypted_text);
    }
    /*
    extended euclidian gcd test
    reference : https://brilliant.org/wiki/extended-euclidean-algorithm/
    */
    public BigInteger gcd(BigInteger m, BigInteger n){
        if(n==0)return m;
        return gcd(n, m%n);
    }
    /*
    Extended Euclidian inverse mod algorithm used to find d
    refernence: http://www-math.ucdenver.edu/~wcherowi/courses/m5410/exeucalg.html
     */
    public BigInteger ExtendedEuclidian(BigInteger n, BigInteger a , BigInteger b, 
        BigInteger aux_p, int pn, List<BigInteger> hist){
            BigInteger aux_p_1 = 0;
            BigInteger aux_p_2 = 0;
            BigInteger q = 0;
            BigInteger q_1 = 0;
            BigInteger q_2 = 0;
            aux_p = 0;
            if(pn>1){
                aux_p_2 = hist.PopAt(0);
                q_2 = hist.PopAt(0);
                aux_p_1 = hist[0];
                q_1 = hist[1];
            }
            aux_p = ((aux_p_2-aux_p_1*q_2)+n)%n;
            aux_p = pn==0 ?  0 : aux_p;
            aux_p = pn==1 ?  1 : aux_p;
            pn++;
            
            try{
            q = (int)(a/(BigInteger)b);
            BigInteger temp=b;
            b = a%b;
            a=temp;
            hist.Add(aux_p);
            hist.Add(q);
            }
            catch(Exception){
                return aux_p;
            }
        return ExtendedEuclidian(n,a,b,aux_p,pn,hist); 
    }
    /*modular exponentation
    https://www.geeksforgeeks.org/modular-exponentiation-power-in-modular-arithmetic/
     */
    static BigInteger mod_exp(BigInteger x, BigInteger y, BigInteger p) 
    { 
        // Initialize result 
        BigInteger res = 1;      
        
        // Update x if it is more  
        // than or equal to p 
        x = x % p;  
    
        while (y > 0) 
        { 
            // If y is odd, multiply  
            // x with result 
            if((y & 1) == 1) 
                res = (res * x) % p; 
    
            // y must be even now 
            // y = y / 2 
            y = y >> 1;  
            x = (x * x) % p;  
        } 
        return res; 
    } 
    /*
    fermat primality test
    reference : https://www.geeksforgeeks.org/primality-test-set-2-fermet-method/
     */
     public bool fermat(BigInteger n){
         BigInteger test = 2;
         bool prime = true;
         while(test < n && prime){
            prime = (BigInteger.Pow(test,(int)n-1) % n) == 1 ? true : false;
            test++;
        }
        return prime;
     }
}
static class ListExtension
    {
        public static T PopAt<T>(this List<T> list, int index)
        {
            T r = list[index];
            list.RemoveAt(index);
            return r;
        }
    }
class FileHelper{
    public void FileOutput(BigInteger str, string filename){
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str.ToString().ToCharArray());
        int numBytesToRead = bytes.Length;

        // Write the byte array to the other FileStream.
        using (FileStream fsNew = new FileStream(filename,
            FileMode.OpenOrCreate, FileAccess.Write))
        {
            fsNew.SetLength(0);
            fsNew.Write(bytes, 0, numBytesToRead);
        }
    }
    public string FileInput(string filename){
        FileStream F = new FileStream(filename, FileMode.OpenOrCreate, 
            FileAccess.ReadWrite);
        byte[] bytes = new byte[F.Length];
        int numBytesToRead = (int)F.Length;
        int numBytesRead = 0;

        while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = F.Read(bytes, numBytesRead, numBytesToRead);
                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }

        string str = "";
        numBytesToRead = bytes.Length;
        for(int i=0;i<numBytesRead;i++){
            str += (char)bytes[i];
        }
    return str;
    }
}