import React from "react";
import { Container, Row, Col, Card } from "react-bootstrap";

const About = () => {
    return (
        <Container className="mt-5">
            <h2 className="text-center mb-4">🛍️ About Abdullah's Fake Store</h2>
            <p className="text-center text-muted mb-5">
                At <strong>Abdullah’s Fake Store</strong>, we bring you the experience of
                a modern e-commerce platform – built entirely for learning React, Bootstrap,
                and Ant Design in a fun way!
            </p>

            {/* Intro Section */}
            <Row className="mb-5">
                <Col md={6}>
                    <h4>Who We Are</h4>
                    <p>
                        We are a trusted online store committed to providing our customers with top-quality products at the best prices.
                        At Abdullah’s Store, we focus on delivering an easy, secure, and enjoyable shopping experience.
                        From trending fashion to must-have accessories, we ensure our collection meets the needs of every shopper.
                        Our mission is simple: to bring quality, affordability, and convenience together – all in one place.

                    </p>
                </Col>

                <Col md={6}>
                    <h4>Our Mission</h4>
                    <p>
                        Our mission is to empower customers with a seamless shopping journey. 🛒
                        We aim to deliver a complete and satisfying experience with every purchase. 💡
                        Our vision is to make online shopping practical, reliable, and exciting. ✨

                    </p>
                </Col>
            </Row>

            {/* Features Section */}
            <h3 className="text-center mb-4">✨ Why Shop With Us?</h3>
            <Row>
                <Col md={4} className="mb-4">
                    <Card className="p-3 text-center shadow h-100">
                        <Card.Body>
                            <h5>🏆 Top-Notch Products</h5>
                            <p>We showcase only high-quality and trending items for your practice store.</p>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={4} className="mb-4">
                    <Card className="p-3 text-center shadow h-100">
                        <Card.Body>
                            <h5>💳 Secure Shopping</h5>
                            <p>Experience the flow of safe and secure e-commerce transactions.</p>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={4} className="mb-4">
                    <Card className="p-3 text-center shadow h-100">
                        <Card.Body>
                            <h5>💰 Best Prices</h5>
                            <p>Our fake store is designed with real-world shopping vibes – at unbeatable prices.</p>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default About;
