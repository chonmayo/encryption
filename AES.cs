//make an Object Type for Key potentially set max length to 128bit
using System;
using Program;
namespace AES{
    class AES{
        const string plaintext = "helloworldhellow";
        const string key = "p3s6v9y$B&E)H+Mb";
        static int[] rcon = new int[10]{2,4,8,16,32,64,128,27,54,108};
        //recursivley encrypt each round and generate round key at end of iteration
        public static void Main(string[] args){
            //Console.WriteLine('a'^'b');
            //create 2d arrays
            byte[,] text = Make2DArray(plaintext.ToCharArray(),4,4);
            byte[,] key_array = Make2DArray(key.ToCharArray(),4,4);
            
            Console.WriteLine(encrypt_round(text,key_array,0));
        }
        //might be easier to pass char arrays than to convert them inside the method
        public static string encrypt_round(byte[,] state, byte[,] round_key, int n){
            n++;
            Console.WriteLine("ROUND: "+n+"\nPLAINTEXT: "+Print2DArray(state,4,4)+"\nKEY: "+Print2DArray(round_key,4,4)+"\n");
            state = SubBytes(state);
            state = ShiftRows(state);
            state = MixColumns(state);
            state = XOrRoundKey(state,round_key);
            
            
            string s  = n==10 ? plaintext: encrypt_round(state,key_expansion(round_key,rcon[n]),n);
            return s;
        }

        static byte[,] ShiftRows(byte[,] state){
            byte[,] output = new byte[4,4];
            for(int row=0;row<4;row++){
                for(int b=0;b<4;b++){
                    output[row,(b-row+4)%4] = state[row,b];
                }
            }
            return output;
        }
        static byte[,] SubBytes(byte[,] state){
            FileHelper f = new FileHelper();
            string raw_vals = f.FileInput("s-box.txt");
            string[] sbox = raw_vals.Split(" ");

            for(int i=0;i<4;i++){
                for(int j=0;j<4;j++){
                byte b = state[i,j];
                string bits = Convert.ToString(b,2).PadLeft(8,'0');
                int x = Convert.ToInt16(bits.Substring(0,4),2);
                int y = Convert.ToInt16(bits.Substring(4,4),2);
                int index = y*16+x;
                state[i,j] = (byte)Convert.ToInt16(sbox[index],16);
                }
            }
            return state;
        }
        static byte[,] MixColumns(byte[,] state){

            byte[,] k = new byte[4,4]{
                {2,3,1,1},
                {1,2,3,1},
                {1,1,2,3},
                {3,1,1,2},
            };

            byte[,] output = new byte[4,4];
            byte sum = 0;
            for(int column=0;column<4;column++){
                for(int b=0;b<4;b++){
                    for(int i=0;i<4;i++){
                        sum = (byte)(sum^(state[b,column]*k[column,i]));
                    }
                    output[b,column] = sum;
                }
            }
            return output;
        }
        static byte[,] key_expansion(byte[,] key, int n){
            byte[,] output = new byte[4,4];

            for(int word=0;word<4;word++){
                for(int b=0;b<4;b++){
                    if(word==0){
                        output[b,word] = (byte)(key[b,word]^key[(b+1)%3,word]^n);
                    }
                    else{
                        output[b,word] = (byte)(key[b,word]^output[b,word-1]);
                    }
                }
            }
            return output;
        }
        //this function computes XOR addition and subtraction
        static byte[,] XOrRoundKey(byte[,] a, byte[,] b){
            byte[,] output = new byte[4,4];
            for(int i=0;i<4;i++){
                for(int j=0;j<4;j++){
                    output[i,j] = (byte)(a[i,j] ^ b[i,j]);
                }
            }
            
            return output;
        }
        static int gmul(int a, int b){
            int product = 0;
            int high_bits;

            for(int i = 0;i<8;i++){
                //if the low bit of b is one
                if((b & 1) == 1){
                    product = product^a;
                }
                //0x80 reproductresents 128, stored to check if a is below 128
                high_bits = (a & 0x80);
                //shift bits one to the left
                a = a << 1;
                //0x1b is 27, 
                if(high_bits == 0x80){
                    a = a^0x1b;
                }
                b = b >> 1;
            }

            return product;
        }
        private static byte[,] Make2DArray(char[] input, int height, int width){
            byte[,] output = new byte[width, height];
            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                {
                    output[h, w] = (byte)input[w * height + h];
                }
            return output;
        }
        private static string Print2DArray(byte[,] input, int height, int width){
            string output = "";

            for (int w = 0; w < width; w++){
                output+="\n";
                for (int h = 0; h < height; h++)
                {
                    output+=(char)input[w,h]+" ";
                }
            }
                return output;
        }
    }
}


/*To create the state array...
For each row in A, multiply by */