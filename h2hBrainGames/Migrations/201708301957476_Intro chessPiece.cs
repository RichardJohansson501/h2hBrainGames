namespace h2hBrainGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntrochessPiece : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChessPieces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Row = c.Int(nullable: false),
                        Column = c.Int(nullable: false),
                        Color = c.Int(nullable: false),
                        Piece = c.Int(nullable: false),
                        ChessGameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChessGames", t => t.ChessGameId, cascadeDelete: true)
                .Index(t => t.ChessGameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChessPieces", "ChessGameId", "dbo.ChessGames");
            DropIndex("dbo.ChessPieces", new[] { "ChessGameId" });
            DropTable("dbo.ChessPieces");
        }
    }
}
