<?php
header("Content-Type: application/json");

// Replace with your database connection credentials
$servername = "localhost";
$username = "root"; 
$password = ""; 
$dbname = "pokedex";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// Create SQL query to get all card information
$sql = "SELECT cardID, cardName, setName, cardRarity FROM cards";

$result = $conn->query($sql);

// Check if any results found
if ($result->num_rows > 0) {
    $cards = array();
    while($row = $result->fetch_assoc()) {
        $cards[] = $row;
    }
    echo json_encode($cards); // Encode all cards as JSON and send response
} else {
    // No results found, send an error message
    $cardData = array(
        "error" => "No cards found in database"
    );
    echo json_encode($cardData);
}

$conn->close();
?>