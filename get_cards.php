<?php
// Database connection
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

// Fetching cards based on card name
$name = $_GET['name'];

$sql = "SELECT * FROM cards WHERE cardName LIKE '%$name%'";
$result = $conn->query($sql);

$rows = array();
while($r = mysqli_fetch_assoc($result)) {
    $rows[] = $r;
}

echo json_encode($rows);

$conn->close();
?>