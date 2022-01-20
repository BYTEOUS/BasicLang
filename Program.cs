using System;
using System.Collections.Generic;

namespace ConsoleApp12
{
    sealed class Token
    {
        public enum TokenType
        {
            Print,
            PrintIn,
            StringLiteral
        }

        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    abstract class ASTNode
    {
        public abstract void Eval();
    }

    sealed class ProgramAst : ASTNode
    {
        public List<ASTNode> Statements { get; set; }

        public ProgramAst()
        {
            Statements = new List<ASTNode>();
        }

        public override void Eval()
        {
            foreach(ASTNode statement in Statements)
                statement.Eval();
        }
    }

    sealed class PrintAst : ASTNode
    {
        public Token Value { get; }

        public PrintAst(Token value)
        {
            Value = value;
        }

        public override void Eval()
        {
            Console.WriteLine(Value.Value);
        }
    }

    class Program
    {
        // Scanner: converts text to array of words
        static string[] Scan(string text)
        {
            List<string> words = new List<string>();

            string current = "";
            foreach(char ch in text)
            {
                // add word
                if (ch == ' ' && current != string.Empty)
                {
                    words.Add(current);
                    current = "";
                }
                else if (!char.IsWhiteSpace(ch))
                    current += ch;
            }

            return words.ToArray();
        }

        // Lexer: converts words to tokens
        static Token[] Lex(string[] words)
        {
            List<Token> tokens = new List<Token>();

            foreach(string word in words)
            {
                //print command
                if (word == "PRINT")
                    tokens.Add(new Token(Token.TokenType.Print, word));
                //string literal
                else if (word.Length >= 2 && word[0] == '\'' && word[word.Length - 1] == '\'')
                    tokens.Add(new Token(Token.TokenType.StringLiteral, word.Replace("'", "")));
                //unexpected token (not whitespace)
                else
                    Console.WriteLine("Unexpected token: " + word);
            }

            return tokens.ToArray();
        }

        // Parser: converts tokens to AST
        static ProgramAst Parse(Token[] tokens)
        {
            ProgramAst program = new ProgramAst();

            if (tokens.Length == 0)
                return null;

            int cursor;
            for(cursor = 0; cursor < tokens.Length; cursor++)
            {
                Token current = tokens[cursor];

                // Print statement
                if (current.Type == Token.TokenType.Print)
                {
                    // next is exists ?
                    if (cursor + 1 < tokens.Length)
                    {
                        Token printVal = tokens[cursor + 1];
                        // next is string ?
                        if(printVal.Type == Token.TokenType.StringLiteral)
                        {
                            cursor++;
                            program.Statements.Add(new PrintAst(printVal));
                        }
                        else
                            Console.WriteLine("Expected string after print, but got token: " + current.Type);
                    }
                }
                else
                    Console.WriteLine("Unexpected token: '" + current.Type + "'");
            }

            return program;
        }

        static void Main(string[] args)
        {
            string code = @"
                PRINT 'Hello' PRINT ','
                PRINT 'World'
            ";

            string[] words = Scan(code);
            Token[] tokens = Lex(words);
            ProgramAst program = Parse(tokens);

            if (program != null)
            {
                foreach (Token token in tokens)
                    Console.WriteLine($"Token: [{token.Type}]");

                //launch
                program.Eval();
            }
        }
    }
}