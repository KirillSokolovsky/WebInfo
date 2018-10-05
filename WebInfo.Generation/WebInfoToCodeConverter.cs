namespace WebInfo.Generation
{
    using WebInfo;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    public class WebInfoToCodeConverter
    {
        private const string InfoStaticClassName = "_Info";
        private const string InfoClassName = "Info";
        private const string LocatorStaticClassName = "_Locator";
        private const string LocatorClassName = "Locator";

        private const string WebInfoAssemblyName = "EmailService.E2E.Tests.Modules.WebInfo";

        public CompilationUnitSyntax GetCompilationUnitForContext(WebContext context)
        {
            var cu = SF.CompilationUnit()
                .WithUsings(
                    SF.List(
                        new List<UsingDirectiveSyntax>
                        {
                            SF.UsingDirective(SF.ParseName(WebInfoAssemblyName)),
                            SF.UsingDirective(SF.ParseName("System.Collections.Generic"))
                        }
                    )
                );

            var webInfoGen = GetClassForWebElement(context);

            cu = cu.AddMembers(webInfoGen.ClassSyntax);

            return cu;
        }

        public WebInfoGenerationData GetClassForWebElement(WebElementInfo info)
        {
            var className = GetClassNameFromElementName(info.Name);
            var propName = className.Substring(1);
            var constInfoCD = GetClassForConstInfo(info);

            var baseClassName = info.GetType().Name;
            var docComment = GetDocCommentWithText(info.Description);

            var infoComment = GetDocCommentWithText("Information about element");

            var infoProperty = SF.PropertyDeclaration(
                    SF.ParseTypeName($"{InfoStaticClassName}.{InfoClassName}"),
                    SF.Identifier(InfoClassName)
                )
                .WithAccessorList(
                    SF.AccessorList(
                        SF.List(
                            new List<AccessorDeclarationSyntax>
                            {
                                SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken)),
                                SF.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken))
                            }
                        )
                    )
                    .WithOpenBraceToken(SF.Token(SyntaxKind.OpenBraceToken))
                    .WithCloseBraceToken(SF.Token(SyntaxKind.CloseBraceToken))
                )
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword).WithLeadingTrivia(SF.Trivia(infoComment)));

            var getInstMd = GetGetInstanceMethod(className);

            var cd = SF.ClassDeclaration(className)
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword).WithLeadingTrivia(SF.Trivia(docComment)))
                .AddBaseListTypes(SF.SimpleBaseType(SF.IdentifierName(baseClassName)))
                .AddMembers(infoProperty, getInstMd, constInfoCD);

            var genData = new WebInfoGenerationData
            {
                ClassName = className,
                PropertyName = propName,
                ClassSyntax = cd,
                Element = info
            };

            FillWithChildrenElementsProperties(genData, out List<WebInfoGenerationData> childrenGens);

            AddCtor(genData, childrenGens);

            FillWithChildrenElementsClasses(genData, childrenGens);

            return genData;
        }
        private void FillWithChildrenElementsProperties(WebInfoGenerationData genData, out List<WebInfoGenerationData> childrenGens)
        {
            childrenGens = null;
            if (!(genData.Element is CombinedWebElementInfo combinedInfo)) return;

            childrenGens = combinedInfo.Elements
                ?.Select(e => GetClassForWebElement(e)).ToList()
                ?? new List<WebInfoGenerationData>();

            if (childrenGens.Count == 0) return;

            var members = new List<MemberDeclarationSyntax>();

            foreach (var childGen in childrenGens)
            {
                var docComment = GetDocCommentWithText(childGen.Element.Description);

                var pd = SF.PropertyDeclaration(
                    SF.IdentifierName(SF.Identifier(childGen.ClassName)),
                    SF.Identifier(childGen.PropertyName)
                )
                .WithAccessorList(
                    SF.AccessorList(
                        SF.List(
                            new List<AccessorDeclarationSyntax>
                            {
                                SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken)),
                                SF.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken))
                            }
                        )
                    )
                    .WithOpenBraceToken(SF.Token(SyntaxKind.OpenBraceToken))
                    .WithCloseBraceToken(SF.Token(SyntaxKind.CloseBraceToken))
                )
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword).WithLeadingTrivia(SF.Trivia(docComment)));

                members.Add(pd);
            }

            var cd = genData.ClassSyntax.AddMembers(members.ToArray());

            genData.ClassSyntax = cd;
        }
        private void AddCtor(WebInfoGenerationData genData, List<WebInfoGenerationData> childrenGens)
        {
            var statements = new List<StatementSyntax>();

            var infoInit = SF.ExpressionStatement(
                SF.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SF.IdentifierName(InfoClassName),
                    SF.ObjectCreationExpression(SF.ParseTypeName($"{InfoStaticClassName}.{InfoClassName}"))
                        .WithArgumentList(SF.ArgumentList())
                )
            );
            statements.Add(infoInit);

            var locatorInit = SF.ExpressionStatement(
                SF.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SF.IdentifierName(LocatorClassName),
                    SF.ObjectCreationExpression(SF.IdentifierName(nameof(WebLocatorInfo)))
                        .WithArgumentList(SF.ArgumentList())
                )
            );
            statements.Add(locatorInit);

            var locatorProps = new List<string>
            {
                nameof(WebLocatorInfo.LocatorType),
                nameof(WebLocatorInfo.LocatorValue),
                nameof(WebLocatorInfo.IsRelative)
            };

            foreach (var locatorProp in locatorProps)
            {
                var st = SF.ExpressionStatement(
                    SF.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SF.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SF.IdentifierName(LocatorClassName),
                            SF.IdentifierName(locatorProp)
                        ),
                        SF.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SF.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SF.IdentifierName(InfoStaticClassName),
                                SF.IdentifierName(LocatorStaticClassName)
                            ),
                            SF.IdentifierName(locatorProp)
                        )
                    )
                );
                statements.Add(st);
            }

            if(childrenGens?.Count > 0)
            {
                var elsInit = SF.ExpressionStatement(
                    SF.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SF.IdentifierName(nameof(CombinedWebElementInfo.Elements)),
                        SF.ObjectCreationExpression(
                            SF.GenericName(SF.Identifier("List"))
                                .WithTypeArgumentList(SF.TypeArgumentList(SF.SingletonSeparatedList<TypeSyntax>(SF.IdentifierName(nameof(WebElementInfo)))))
                        )
                            .WithArgumentList(SF.ArgumentList())
                    )
                );
                statements.Add(elsInit);

                foreach (var childGen in childrenGens)
                {
                    var propInit = SF.ExpressionStatement(
                        SF.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SF.IdentifierName(childGen.PropertyName),
                            SF.ObjectCreationExpression(SF.IdentifierName(childGen.ClassName))
                                .WithArgumentList(SF.ArgumentList())
                        )
                    );
                    statements.Add(propInit);

                    var parentSt = SF.ExpressionStatement(
                        SF.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SF.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SF.IdentifierName(childGen.PropertyName),
                                SF.IdentifierName(nameof(WebElementInfo.Parent))
                            ),
                            SF.ThisExpression()
                        )
                    );
                    statements.Add(parentSt);

                    var elsAddSt = SF.ExpressionStatement(
                        SF.InvocationExpression(
                            SF.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SF.IdentifierName(nameof(CombinedWebElementInfo.Elements)),
                                SF.IdentifierName("Add")
                            )
                        ).WithArgumentList(
                            SF.ArgumentList(
                                SF.SingletonSeparatedList(SF.Argument(SF.IdentifierName(childGen.PropertyName)))
                            )
                        )
                    );
                    statements.Add(elsAddSt);
                }
            }

            var summary = GetDocCommentWithText(genData.Element.Description);

            var ctor = SF.ConstructorDeclaration(SF.Identifier(genData.ClassName))
                .WithBody(SF.Block(statements))
                .WithModifiers(SF.TokenList(SF.Token(SyntaxKind.PublicKeyword).WithLeadingTrivia(SF.Trivia(summary))));

            var cd = genData.ClassSyntax.AddMembers(ctor);
            genData.ClassSyntax = cd;
        }

        private void FillWithChildrenElementsClasses(WebInfoGenerationData genData, List<WebInfoGenerationData> childrenGens)
        {
            if (childrenGens == null || childrenGens.Count == 0) return;

            foreach (var childGen in childrenGens)
            {
                genData.ClassSyntax = genData.ClassSyntax.AddMembers(childGen.ClassSyntax);
            }
        }

        private ClassDeclarationSyntax GetClassForConstInfo(WebElementInfo info)
        {
            var infoMembers = new List<MemberDeclarationSyntax>();
            var infoMembersList = new List<(string type, string fName, ExpressionSyntax expr)>
            {
                ("string", nameof(info.ElementType), GetESForValue(info.ElementType)),
                ("string", nameof(info.Name), GetESForValue(info.Name)),
                ("string", nameof(info.Description), GetESForValue(info.Description)),
                ("bool", nameof(info.IsKey), GetESForValue(info.IsKey)),
                ("string", nameof(info.InnerKey), GetESForValue(info.InnerKey)),
            };

            foreach (var infoMemberItem in infoMembersList)
            {

                var fd = SF.FieldDeclaration(
                    SF.VariableDeclaration(SF.ParseTypeName(infoMemberItem.type))
                        .AddVariables(SF.VariableDeclarator(SF.Identifier(infoMemberItem.fName))
                            .WithInitializer(SF.EqualsValueClause(infoMemberItem.expr))
                        )
                )
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword), SF.Token(SyntaxKind.ConstKeyword));
                infoMembers.Add(fd);
            }

            var locatorStaticCD = GetClassForConstLocatorInInfo(info.Locator);
            infoMembers.Add(locatorStaticCD);

            var infoCD = GetClassForInfo(info);
            infoMembers.Add(infoCD);

            var locatorCD = GetClassForLocatorInInfo(info.Locator);
            infoMembers.Add(locatorCD);

            var infoStaticCD = SF.ClassDeclaration(InfoStaticClassName)
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword), SF.Token(SyntaxKind.StaticKeyword))
                .AddMembers(infoMembers.ToArray());

            return infoStaticCD;
        }
        private ClassDeclarationSyntax GetClassForConstLocatorInInfo(WebLocatorInfo locatorInfo)
        {
            var infoMembers = new List<MemberDeclarationSyntax>();
            var infoMembersList = new List<(string type, string fName, ExpressionSyntax expr)>
            {
                (nameof(WebLocatorType), nameof(locatorInfo.LocatorType), GetESForValue(locatorInfo.LocatorType)),
                ("string", nameof(locatorInfo.LocatorValue), GetESForValue(locatorInfo.LocatorValue)),
                ("bool", nameof(locatorInfo.IsRelative), GetESForValue(locatorInfo.IsRelative))
            };

            foreach (var infoMemberItem in infoMembersList)
            {

                var fd = SF.FieldDeclaration(
                    SF.VariableDeclaration(SF.ParseTypeName(infoMemberItem.type))
                        .AddVariables(SF.VariableDeclarator(SF.Identifier(infoMemberItem.fName))
                            .WithInitializer(SF.EqualsValueClause(infoMemberItem.expr))
                        )
                )
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword), SF.Token(SyntaxKind.ConstKeyword));
                infoMembers.Add(fd);
            }



            var infoCD = SF.ClassDeclaration(LocatorStaticClassName)
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword), SF.Token(SyntaxKind.StaticKeyword))
                .AddMembers(infoMembers.ToArray());

            return infoCD;
        }

        private ClassDeclarationSyntax GetClassForInfo(WebElementInfo info)
        {
            var infoMembers = new List<MemberDeclarationSyntax>();
            var infoMembersList = new List<(string type, string fName, string value)>
            {
                ("string", nameof(info.ElementType), info.ElementType),
                ("string", nameof(info.Name), info.Name),
                ("string", nameof(info.Description), info.Description),
                ("bool", nameof(info.IsKey), $"{info.IsKey}"),
                ("string", nameof(info.InnerKey), $"{info.InnerKey ?? "null"}"),
                (LocatorClassName, LocatorClassName, "Element Locator values")
            };


            var newLineToken = SF.Token(default(SyntaxTriviaList),
                    SyntaxKind.XmlTextLiteralNewLineToken,
                    Environment.NewLine, Environment.NewLine, default(SyntaxTriviaList));

            var docCommentToken = SF.Token(SF.TriviaList(
                SF.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, "///")),
                SyntaxKind.XmlTextLiteralToken, " ", " ", default(SyntaxTriviaList));

            var endNode = SF.XmlText(
                  SF.TokenList(SF.Token(default(SyntaxTriviaList), SyntaxKind.XmlTextLiteralNewLineToken,
                   Environment.NewLine, Environment.NewLine, default(SyntaxTriviaList))));


            foreach (var infoMemberItem in infoMembersList)
            {
                var docComment = GetDocCommentWithText(infoMemberItem.value);

                var fd = SF.FieldDeclaration(
                    SF.VariableDeclaration(SF.ParseTypeName(infoMemberItem.type))
                        .AddVariables(SF.VariableDeclarator(SF.Identifier(infoMemberItem.fName)))
                )
                .AddModifiers(
                    SF.Token(SF.TriviaList(), SyntaxKind.PublicKeyword, SF.TriviaList())
                        .WithLeadingTrivia(SF.Trivia(docComment))
                );

                infoMembers.Add(fd);
            }

            var infoCD = SF.ClassDeclaration(InfoClassName)
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword))
                .AddMembers(infoMembers.ToArray());

            return infoCD;
        }
        private ClassDeclarationSyntax GetClassForLocatorInInfo(WebLocatorInfo locatorInfo)
        {
            var infoMembers = new List<MemberDeclarationSyntax>();
            var infoMembersList = new List<(string type, string fName, string value)>
            {
                (nameof(WebLocatorType), nameof(locatorInfo.LocatorType), $"{nameof(WebLocatorType)}.{locatorInfo.LocatorType}"),
                ("string", nameof(locatorInfo.LocatorValue), locatorInfo.LocatorValue),
                ("bool", nameof(locatorInfo.IsRelative), $"{locatorInfo.IsRelative}"),
            };

            foreach (var infoMemberItem in infoMembersList)
            {
                var docComment = GetDocCommentWithText(infoMemberItem.value);

                var fd = SF.FieldDeclaration(
                    SF.VariableDeclaration(SF.ParseTypeName(infoMemberItem.type))
                        .AddVariables(SF.VariableDeclarator(SF.Identifier(infoMemberItem.fName))
                    )
                )
                .AddModifiers(
                    SF.Token(SF.TriviaList(), SyntaxKind.PublicKeyword, SF.TriviaList())
                        .WithLeadingTrivia(SF.Trivia(docComment))
                );
                infoMembers.Add(fd);
            }

            var infoCD = SF.ClassDeclaration(LocatorClassName)
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword))
                .AddMembers(infoMembers.ToArray());

            return infoCD;
        }

        private MethodDeclarationSyntax GetGetInstanceMethod(string className)
        {
            var comment = GetDocCommentWithText("Get new element instance");

            var md = SF.MethodDeclaration(
                    SF.IdentifierName(className),
                    SF.Identifier("GetInstance")
                )
                .AddModifiers(SF.Token(SyntaxKind.PublicKeyword).WithLeadingTrivia(SF.Trivia(comment)), SF.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(
                    SF.Parameter(SF.Identifier("parentElement"))
                        .WithType(SF.IdentifierName(nameof(CombinedWebElementInfo)))
                )
                .WithExpressionBody(
                    SF.ArrowExpressionClause(
                        SF.ObjectCreationExpression(SF.IdentifierName(className))
                        .WithInitializer(
                            SF.InitializerExpression(
                                SyntaxKind.ObjectInitializerExpression
                            )
                            .AddExpressions(
                                SF.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SF.IdentifierName(nameof(WebElementInfo.Parent)), SF.IdentifierName("parentElement"))
                            )
                        )
                    )
                )
                .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken));

            return md;
        }

        private ExpressionSyntax GetESForValue(object val)
        {
            if (val is Enum)
            {
                return SF.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SF.IdentifierName(val.GetType().Name),
                    SF.IdentifierName($"{val}")
                    );
            }
            else if (val is string str)
            {
                return SF.ParseExpression(val == null ? "null" : $"\"{str}\"");
            }
            else if (val is bool)
            {
                return SF.ParseExpression(val?.ToString().ToLower());
            }
            return SF.ParseExpression(val?.ToString() ?? "null");
        }
        private string GetClassNameFromElementName(string elementName)
        {
            var words = elementName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (words.Count == 1)
            {
                return $"_{words.First()}";
            }

            return $"_{string.Join("", words.Take(words.Count - 1))}_{words.Last()}";
        }
        private DocumentationCommentTriviaSyntax GetDocCommentWithText(string text)
        {
            var summaryComment = SF.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia)
                    .AddContent(
                        SF.XmlText(
                            SF.Token(SyntaxKind.XmlTextLiteralToken)
                                .WithLeadingTrivia(
                                    SF.TriviaList(SF.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, "///"))
                                )
                        ),
                        SF.XmlElement(
                            SF.XmlElementStartTag(SF.XmlName(SF.Identifier(@"summary"))),
                            SF.XmlElementEndTag(SF.XmlName(SF.Identifier(@"summary")))
                        )
                            .AddContent(
                                SF.XmlText(
                                    SF.Token(SyntaxKind.XmlTextLiteralNewLineToken),
                                    SF.Token(
                                        SF.TriviaList(SF.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, "///")),
                                        SyntaxKind.XmlTextLiteralToken,
                                        $" {text}",
                                        $" {text}",
                                        SF.TriviaList()
                                    ),
                                    SF.Token(SyntaxKind.XmlTextLiteralNewLineToken),
                                    SF.Token(SyntaxKind.XmlTextLiteralToken)
                                        .WithLeadingTrivia(
                                            SF.TriviaList(SF.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, "///"))
                                        )
                                )
                            ),
                        SF.XmlText(SF.XmlTextNewLine(SF.TriviaList(), Environment.NewLine, Environment.NewLine, SF.TriviaList()))
                    )
                    .WithEndOfComment(SF.Token(SyntaxKind.EndOfDocumentationCommentToken));

            return summaryComment;

        }
    }
}
